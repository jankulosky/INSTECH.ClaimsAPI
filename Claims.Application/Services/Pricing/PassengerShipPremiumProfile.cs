using Claims.Enums;

namespace Claims.Application.Services.Pricing;

public sealed class PassengerShipPremiumProfile : IPremiumProfile
{
    public bool AppliesTo(CoverType coverType) => coverType == CoverType.PassengerShip;
    public int Priority => 100;
    public decimal TypeMultiplier => PremiumPolicyConstants.MultiplierPassengerShip;
    public decimal SecondBandDiscount => PremiumPolicyConstants.DiscountSecondBandDefault;
    public decimal ThirdBandDiscount => PremiumPolicyConstants.DiscountThirdBandDefault;
}
