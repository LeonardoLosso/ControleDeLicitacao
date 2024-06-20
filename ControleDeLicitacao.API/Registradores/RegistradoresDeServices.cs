using ControleDeLicitacao.App.Mappings;
using ControleDeLicitacao.App.Services;
using ControleDeLicitacao.Infrastructure.Persistence.Contexto;
using ControleDeLicitacao.Infrastructure.Persistence.Interface;
using ControleDeLicitacao.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControleDeLicitacao.API.Registradores;

public static class RegistradoresDeServices
{

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        //Services
        services.AddScoped<EntidadeService>();
        services.AddScoped<ItemService>();

        //Repo e context
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<DbContext, EntidadeContext>();
        services.AddScoped<ItemRepository>();

        //Mappers
        services.AddAutoMapper(typeof(EntidadeMapping).Assembly);
        services.AddAutoMapper(typeof(ItemMapping).Assembly);

        return services;
    }
}
