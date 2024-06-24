using ControleDeLicitacao.App.Mappings;
using ControleDeLicitacao.App.Services.Cadastros;
using ControleDeLicitacao.App.Services.Cadastros.User;
using ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using ControleDeLicitacao.Infrastructure.Persistence.Interface;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        //falta criar authorization e authentication

        services
            .AddIdentity<Usuario, IdentityRole<int>>()
            .AddEntityFrameworkStores<UsuarioContext>()
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
