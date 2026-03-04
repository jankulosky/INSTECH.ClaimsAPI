using Claims.Application.Services;
using Claims.Application.Services.Interfaces;
using Claims.Application.Services.Pricing;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Application.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IPremiumProfile, YachtPremiumProfile>();
        services.AddSingleton<IPremiumProfile, PassengerShipPremiumProfile>();
        services.AddSingleton<IPremiumProfile, TankerPremiumProfile>();
        services.AddSingleton<IPremiumProfile, DefaultPremiumProfile>();
        services.AddSingleton<IPremiumCalculator, PremiumCalculator>();
        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<ICoverService, CoverService>();
        return services;
    }
}
