using Claims.Application.Exceptions;
using Claims.Enums;

namespace Claims.Application.Services.Pricing;

public sealed class PremiumCalculator : IPremiumCalculator
{
    private const decimal BaseDayRate = 1250m;
    private readonly IReadOnlyCollection<IPremiumProfile> _profiles;

    public PremiumCalculator(IEnumerable<IPremiumProfile> profiles)
    {
        _profiles = profiles.ToArray();
    }

    public decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        if (endDate <= startDate)
        {
            throw new ValidationException("EndDate must be greater than StartDate.");
        }

        var totalDays = (int)(endDate.Date - startDate.Date).TotalDays;
        var profile = _profiles
            .Where(x => x.AppliesTo(coverType))
            .OrderByDescending(x => x.Priority)
            .FirstOrDefault()
            ?? throw new InvalidOperationException($"No premium profile registered for cover type '{coverType}'.");

        var dailyBaseRate = BaseDayRate * profile.TypeMultiplier;
        var firstBandDays = Math.Min(30, totalDays);
        var secondBandDays = Math.Min(150, Math.Max(0, totalDays - 30));
        var thirdBandDays = Math.Max(0, totalDays - 180);

        var firstBandPremium = firstBandDays * dailyBaseRate;
        var secondBandPremium = secondBandDays * dailyBaseRate * (1m - profile.SecondBandDiscount);
        var thirdBandPremium = thirdBandDays * dailyBaseRate * (1m - profile.ThirdBandDiscount);

        return firstBandPremium + secondBandPremium + thirdBandPremium;
    }
}
