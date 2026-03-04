using Claims.Enums;

namespace Claims.Application.Services.Pricing;

public interface IPremiumCalculator
{
    decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType);
}
