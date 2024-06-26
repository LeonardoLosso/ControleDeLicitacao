using ControleDeLicitacao.App.Mappings;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.App.Services.Cadastros.User;
using ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using ControleDeLicitacao.Infrastructure.Persistence.Interface;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ControleDeLicitacao.API.Registradores;

public static class Registradores
{
    public static IServiceCollection AddContexts(this IServiceCollection services, string connString)
    {
        services.AddDbContext<EntidadeContext>(opts => opts.UseSqlServer(connString));

        services.AddDbContext<ItemContext>(opts => opts.UseSqlServer(connString));

        services.AddDbContext<UsuarioContext>(opts => opts.UseSqlServer(connString));

        return services;
    }

    public static IServiceCollection AddUserConfig(this IServiceCollection services)
    {

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes("F4ça)uNã0FaÇ4.4T3n74t1VAnÃO3xI27E")),

                ValidateAudience = false,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero
            };
        });

        services
            .AddIdentity<Usuario, IdentityRole<int>>()
            .AddEntityFrameworkStores<UsuarioContext>()
            .AddUserStore<CustomUserStore>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<EntidadeService>();
        services.AddScoped<ItemService>();
        services.AddScoped<UsuarioService>();
        services.AddScoped<TokenService>();

        return services;
    }

    public static IServiceCollection AddInfraRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<DbContext, EntidadeContext>();
        services.AddScoped<ItemRepository>();

        return services;
    }

    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(EntidadeMapping).Assembly);
        services.AddAutoMapper(typeof(ItemMapping).Assembly);
        services.AddAutoMapper(typeof(UsuarioMapping).Assembly);

        return services;
    }

    public static IServiceCollection AddOthersConfig(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
        });

        services.AddControllers().AddNewtonsoftJson();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
