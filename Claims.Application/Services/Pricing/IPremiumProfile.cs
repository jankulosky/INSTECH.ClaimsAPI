using Claims.Enums;

namespace Claims.Application.Services.Pricing;

public interface IPremiumProfile
{
    bool AppliesTo(CoverType coverType);
    int Priority { get; }
    decimal TypeMultiplier { get; }
    decimal SecondBandDiscount { get; }
    decimal ThirdBandDiscount { get; }
}
