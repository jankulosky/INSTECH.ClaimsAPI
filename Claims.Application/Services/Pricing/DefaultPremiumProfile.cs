using Claims.Enums;

namespace Claims.Application.Services.Pricing;

public sealed class DefaultPremiumProfile : IPremiumProfile
{
    public bool AppliesTo(CoverType coverType) => true;
    public int Priority => 0;
    public decimal TypeMultiplier => 1.3m;
    public decimal SecondBandDiscount => 0.02m;
    public decimal ThirdBandDiscount => 0.03m;
}
