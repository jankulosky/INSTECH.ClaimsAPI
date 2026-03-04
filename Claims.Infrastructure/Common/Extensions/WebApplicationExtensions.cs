using Claims.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Claims.Infrastructure.Common.Extensions;

public static class WebApplicationExtensions
{
    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        var shouldApply = app.Configuration.GetValue<bool?>("Database:ApplyMigrationsOnStartup")
            ?? app.Environment.IsDevelopment();
        if (!shouldApply)
        {
            return;
        }

        var maxRetries = app.Configuration.GetValue("Database:MigrationMaxRetries", 5);
        var initialDelaySeconds = app.Configuration.GetValue("Database:MigrationInitialDelaySeconds", 2);
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("StartupMigration");

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ClaimsDbContext>();
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Database migration succeeded on attempt {Attempt}.", attempt);
                return;
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                var delay = TimeSpan.FromSeconds(initialDelaySeconds * Math.Pow(2, attempt - 1));
                logger.LogWarning(
                    ex,
                    "Database migration attempt {Attempt}/{MaxRetries} failed. Retrying in {Delay}.",
                    attempt,
                    maxRetries,
                    delay);

                await Task.Delay(delay);
            }
        }

        throw new InvalidOperationException(
            $"Database migration failed after {maxRetries} attempts. Check database availability and configuration.");
    }
}
