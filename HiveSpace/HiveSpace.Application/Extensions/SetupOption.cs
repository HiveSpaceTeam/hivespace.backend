using HiveSpace.Application.Models.AppSettings;
using HiveSpace.Commons.Models;

namespace HiveSpace.Application.Extensions;

public static class SetupOption
{
    public static IServiceCollection AddSetupOption(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSetting>(configuration.GetSection("JwtSetting"));
        services.Configure<IdentityServerJwtSetting>(configuration.GetSection("IdentityServerJwtSetting"));
        services.Configure<AppConfig>(configuration.GetSection(nameof(AppConfig)));
        services.Configure<RedisOption>(configuration.GetSection("Redis"));
        return services;
    }
}
