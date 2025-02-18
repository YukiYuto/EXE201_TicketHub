using StackExchange.Redis;
using TicketHub.DataAccess.IRepository;
using TicketHub.DataAccess.Repository;
using TicketHub.Services.IService;
using TicketHub.Services.Service;

namespace TicketHub.API.Extension;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services,
        ConfigurationManager builderConfiguration)
    {
        // Đọc chuỗi kết nối Redis từ file cấu hình
        var redisConnectionString = builderConfiguration.GetValue<string>("Redis:ConnectionString");
        // Đăng ký IConnectionMultiplexer
        var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString!);
        services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRedisService, RedisService>();
        return services;
    }
}