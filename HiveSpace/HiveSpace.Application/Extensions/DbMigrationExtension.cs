using HiveSpace.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiveSpace.Application.Extensions;

public static class DbMigrationExtension
{
    public static IHost Migrate(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<HiveSpaceDbContext>>();
        var dbContext = services.GetRequiredService<HiveSpaceDbContext>();
        try
        {
            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                foreach (var migration in pendingMigrations)
                {
                    logger.LogInformation($"Pending migration = {migration}");
                }
            }
            else
            {
                logger.LogInformation("No pending migration");
            }

            logger.LogInformation("Start migrating");
            dbContext.Database.Migrate();
            logger.LogInformation("Migration completed");

            var appliedMigrations = dbContext.Database.GetAppliedMigrations().ToList();
            logger.LogInformation("Migration history");
            foreach (var migration in appliedMigrations)
            {
                logger.LogInformation(migration);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");
        }
        return host;
    }
}
