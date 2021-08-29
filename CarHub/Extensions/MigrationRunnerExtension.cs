using System.Threading.Tasks;
using CarHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CarHub.Extensions
{
    public static class MigrationRunnerExtension
    {
        public static async Task MigrateDatabase(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("MigrationRunner");
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

            logger.LogInformation("Perfoming database migration...");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Migration completed successfully");
        }
    }
}
