using Claims.Enums;

namespace Claims.Application.Services.Pricing;

public sealed class YachtPremiumProfile : IPremiumProfile
{
    public bool AppliesTo(CoverType coverType) => coverType == CoverType.Yacht;
    public int Priority => 100;
    public decimal TypeMultiplier => PremiumPolicyConstants.MultiplierYacht;
    public decimal SecondBandDiscount => PremiumPolicyConstants.DiscountSecondBandYacht;
    public decimal ThirdBandDiscount => PremiumPolicyConstants.DiscountThirdBandYacht;
}
