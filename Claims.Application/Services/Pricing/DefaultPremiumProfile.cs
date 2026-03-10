using Claims.Enums;

namespace Claims.Application.Services.Pricing;

public sealed class DefaultPremiumProfile : IPremiumProfile
{
    public bool AppliesTo(CoverType coverType) => true;
    public int Priority => 0;
    public decimal TypeMultiplier => PremiumPolicyConstants.MultiplierDefault;
    public decimal SecondBandDiscount => PremiumPolicyConstants.DiscountSecondBandDefault;
    public decimal ThirdBandDiscount => PremiumPolicyConstants.DiscountThirdBandDefault;
}
