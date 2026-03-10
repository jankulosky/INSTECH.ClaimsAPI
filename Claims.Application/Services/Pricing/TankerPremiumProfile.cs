using Claims.Enums;

namespace Claims.Application.Services.Pricing;

public sealed class TankerPremiumProfile : IPremiumProfile
{
    public bool AppliesTo(CoverType coverType) => coverType == CoverType.Tanker;
    public int Priority => 100;
    public decimal TypeMultiplier => PremiumPolicyConstants.MultiplierTanker;
    public decimal SecondBandDiscount => PremiumPolicyConstants.DiscountSecondBandDefault;
    public decimal ThirdBandDiscount => PremiumPolicyConstants.DiscountThirdBandDefault;
}
