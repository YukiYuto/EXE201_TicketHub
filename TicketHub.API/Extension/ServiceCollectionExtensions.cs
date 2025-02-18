using TicketHub.DataAccess.IRepository;
using TicketHub.DataAccess.Repository;
using TicketHub.Services.IService;
using TicketHub.Services.Service;

namespace TicketHub.API.Extension;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services,ConfigurationManager builderConfiguration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}