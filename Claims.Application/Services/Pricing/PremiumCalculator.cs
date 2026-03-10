using Claims.Application.Exceptions;
using Claims.Enums;

namespace Claims.Application.Services.Pricing;

public sealed class PremiumCalculator : IPremiumCalculator
{
    private readonly IReadOnlyCollection<IPremiumProfile> _profiles;

    public PremiumCalculator(IEnumerable<IPremiumProfile> profiles)
    {
        _profiles = profiles.ToArray();
    }

    /// <summary>
    /// Computes premium over three progressive day bands:
    /// first 30 days (no discount), next 150 days (second-band discount),
    /// and remaining days (third-band discount). Time-of-day is ignored.
    /// </summary>
    public decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        if (endDate <= startDate)
        {
            throw new ValidationException("EndDate must be greater than StartDate.");
        }

        var totalDays = DateOnly.FromDateTime(endDate).DayNumber - DateOnly.FromDateTime(startDate).DayNumber;
        var profile = _profiles
            .Where(x => x.AppliesTo(coverType))
            .OrderByDescending(x => x.Priority)
            .FirstOrDefault()
            ?? throw new InvalidOperationException($"No premium profile registered for cover type '{coverType}'.");

        var dailyBaseRate = PremiumPolicyConstants.BaseDayRate * profile.TypeMultiplier;
        var firstBandDays = GetFirstBandDays(totalDays);
        var secondBandDays = GetSecondBandDays(totalDays);
        var thirdBandDays = GetThirdBandDays(totalDays);

        var firstBandPremium = CalculateBandPremium(firstBandDays, dailyBaseRate, 0m);
        var secondBandPremium = CalculateBandPremium(secondBandDays, dailyBaseRate, profile.SecondBandDiscount);
        var thirdBandPremium = CalculateBandPremium(thirdBandDays, dailyBaseRate, profile.ThirdBandDiscount);

        return firstBandPremium + secondBandPremium + thirdBandPremium;
    }

    private static int GetFirstBandDays(int totalDays) =>
        Math.Min(PremiumPolicyConstants.FirstBandDays, totalDays);

    private static int GetSecondBandDays(int totalDays) =>
        Math.Min(
            PremiumPolicyConstants.SecondBandDays,
            Math.Max(0, totalDays - PremiumPolicyConstants.FirstBandDays));

    private static int GetThirdBandDays(int totalDays) =>
        Math.Max(0, totalDays - PremiumPolicyConstants.FirstBandDays - PremiumPolicyConstants.SecondBandDays);

    private static decimal CalculateBandPremium(int days, decimal dailyBaseRate, decimal discount) =>
        days * dailyBaseRate * (1m - discount);
}
