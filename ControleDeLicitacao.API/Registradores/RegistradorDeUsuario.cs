using ControleDeLicitacao.Domain.Entities.Cadastros.Usuario;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using Microsoft.AspNetCore.Identity;

namespace ControleDeLicitacao.API.Registradores
{
    public static class RegistradorDeUsuario
    {

        public static IServiceCollection AddUserConfig(this IServiceCollection services)
        {
            services
                .AddIdentity<Usuario, IdentityRole<int>>()
                .AddEntityFrameworkStores<UsuarioContext>()
                .AddDefaultTokenProviders();

            return services;
        }
        //public static IServiceCollection AddUserServices(this IServiceCollection services)
        //{

        //    services.AddScoped<UsuarioService>();
        //    services.AddScoped<TokenService>();

        //    return services;
        //}
    }
}
