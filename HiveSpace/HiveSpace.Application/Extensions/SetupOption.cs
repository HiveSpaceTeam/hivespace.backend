using HiveSpace.Application.Models.AppSettings;
using HiveSpace.Commons.Models;

namespace HiveSpace.Application.Extensions;

public static class SetupOption
{
    public static IServiceCollection AddSetupOption(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        services.Configure<AppConfig>(configuration.GetSection(nameof(AppConfig)));
        return services;
    }
}
