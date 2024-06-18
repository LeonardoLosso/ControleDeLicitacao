//using ControleDeLicitacao.API.Services;
//using ControleDeLicitacao.API.User;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;

//namespace ControleDeLicitacao.API.Registradores
//{
//    public static class RegistradorDeUsuario
//    {

//        public static IServiceCollection AddUserDbContext(this IServiceCollection services, string connectionString)
//        {
//            services
//                .AddDbContext<UsuarioDbContext>
//                (opts =>
//                {
//                    opts.UseSqlServer(connectionString);
//                });

//            return services;
//        }
//        public static IServiceCollection AddUserConfig(this IServiceCollection services)
//        {
//            services
//                .AddIdentity<Usuario, IdentityRole>()
//                .AddEntityFrameworkStores<UsuarioDbContext>()
//                .AddDefaultTokenProviders();

//            return services;
//        }
//        public static IServiceCollection AddUserServices(this IServiceCollection services)
//        {

//            services.AddScoped<UsuarioService>();
//            services.AddScoped<TokenService>();

//            return services;
//        }
//    }
//}
