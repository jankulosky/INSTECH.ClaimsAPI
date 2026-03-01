using Claims.Auditing;
using Claims.Common.Constants;
using Claims.Data;
using Claims.Repositories;
using Claims.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Claims.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConfigurationConstants.PrimaryDatabaseConnectionName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Missing connection string for '{ConfigurationConstants.PrimaryDatabaseConnectionName}'.");
        }
        
        if (connectionString.Contains('<') || connectionString.Contains('>'))
        {
            throw new InvalidOperationException(
                $"Connection string '{ConfigurationConstants.PrimaryDatabaseConnectionName}' still contains template placeholders. " +
                "Set a real value in environment variables, user-secrets, or appsettings.Development.json.");
        }

        services.AddDbContext<ClaimsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null)));
        services.AddSingleton<IAuditQueue, AuditQueue>();
        services.AddHostedService<AuditBackgroundService>();
        services.AddScoped<IClaimRepository, ClaimRepository>();
        services.AddScoped<ICoverRepository, CoverRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();

        return services;
    }
}
