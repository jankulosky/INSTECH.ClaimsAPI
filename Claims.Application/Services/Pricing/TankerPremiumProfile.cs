using Claims.Enums;

namespace Claims.Application.Services.Pricing;

public sealed class TankerPremiumProfile : IPremiumProfile
{
    public bool AppliesTo(CoverType coverType) => coverType == CoverType.Tanker;
    public int Priority => 100;
    public decimal TypeMultiplier => 1.5m;
    public decimal SecondBandDiscount => 0.02m;
    public decimal ThirdBandDiscount => 0.03m;
}
