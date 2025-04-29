using Microsoft.EntityFrameworkCore;
using HiveSpace.Domain.Repositories;
using HiveSpace.Infrastructure;
using HiveSpace.Infrastructure.Repositories;
using HiveSpace.Domain.SeedWork;
using HiveSpace.Infrastructure.Interceptors;

namespace HiveSpace.Application.Extensions;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigreDatabaseContext(configuration);
        services.ConfigureRepositories();
        return services;
    }

    public static IServiceCollection ConfigreDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<HiveSpaceDbContext>((sp, options) =>
            options.UseNpgsql(configuration.GetSection("Postgres:ConnectionString").Value, b => b.MigrationsAssembly("HiveSpace.Application"))
                   .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>(), sp.GetRequiredService<UpdateAuditableInterceptor>()));
        return services;
    }

    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<ISkuRepository, SkuRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        return services;
    }
}
