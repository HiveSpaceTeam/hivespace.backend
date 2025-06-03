using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using HiveSpace.Infrastructure;

namespace HiveSpace.Application.ContextFactory;

public class HiveSpaceDbContextFactory : IDesignTimeDbContextFactory<HiveSpaceDbContext>
{
    public HiveSpaceDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile("/app/secrets/appsettings.secrets.json", optional: true)
           .AddEnvironmentVariables()
            //.AddJsonFile("config/appsettings.json", optional: true)
            .Build();
        Console.WriteLine(environment);
        var optionsBuilder = new DbContextOptionsBuilder<HiveSpaceDbContext>();
        var connectionString = configuration.GetSection("Postgres:ConnectionString").Value;
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("HiveSpace.Application"));

        return new HiveSpaceDbContext(optionsBuilder.Options, configuration);
    }

    //public static NichoShopDbContext CreateDbContext()
    //{
        
    //}
}