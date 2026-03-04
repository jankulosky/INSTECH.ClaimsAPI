using Claims.Enums;

namespace Claims.Application.Services.Pricing;

public sealed class YachtPremiumProfile : IPremiumProfile
{
    public bool AppliesTo(CoverType coverType) => coverType == CoverType.Yacht;
    public int Priority => 100;
    public decimal TypeMultiplier => 1.1m;
    public decimal SecondBandDiscount => 0.05m;
    public decimal ThirdBandDiscount => 0.08m;
}
