using ControleDeLicitacao.App.Mappings;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.App.Services.Cadastros.User;
using ControleDeLicitacao.App.Services.Documentos.Ata;
using ControleDeLicitacao.App.Services.Logger;
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

        services.AddDbContext<AtaContext>(opts => opts.UseSqlServer(connString));

        return services;
    }

    public static IServiceCollection AddUserConfig(this IServiceCollection services)
    {

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes("F4ca)uNa0FaC4.4T3n74t1VAnAO3xI27E")),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

        
        services
            .AddIdentity<Usuario, IdentityRole<int>>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
            })
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
        services.AddSingleton<TokenService>();
        services.AddScoped<LogInfoService>();
        services.AddScoped<AtaService>();

        return services;
    }

    public static IServiceCollection AddInfraRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<DbContext, EntidadeContext>();
        services.AddScoped<ItemRepository>();
        services.AddScoped<AtaRepository>();

        return services;
    }

    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(EntidadeMapping).Assembly);
        services.AddAutoMapper(typeof(ItemMapping).Assembly);
        services.AddAutoMapper(typeof(UsuarioMapping).Assembly);
        services.AddAutoMapper(typeof(AtaMapping).Assembly);

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
                    //.AllowCredentials();
                });
        });

        services.AddControllers().AddNewtonsoftJson();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
