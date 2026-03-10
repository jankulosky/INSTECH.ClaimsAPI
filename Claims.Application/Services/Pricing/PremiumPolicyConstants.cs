namespace Claims.Application.Services.Pricing;

public static class PremiumPolicyConstants
{
    public const decimal BaseDayRate = 1250m;
    public const int FirstBandDays = 30;
    public const int SecondBandDays = 150;

    public const decimal MultiplierYacht = 1.1m;
    public const decimal MultiplierPassengerShip = 1.2m;
    public const decimal MultiplierTanker = 1.5m;
    public const decimal MultiplierDefault = 1.3m;

    public const decimal DiscountSecondBandYacht = 0.05m;
    public const decimal DiscountSecondBandDefault = 0.02m;
    public const decimal DiscountThirdBandYacht = 0.08m;
    public const decimal DiscountThirdBandDefault = 0.03m;
}
