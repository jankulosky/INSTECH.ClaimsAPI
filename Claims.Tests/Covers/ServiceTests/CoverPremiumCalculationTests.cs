using Claims.Enums;
using Claims.Exceptions;
using Claims.Services;
using Xunit;

namespace Claims.Tests;

public class CoverPremiumCalculationTests
{
    [Fact]
    public void PremiumForYachtUsesProgressiveDiscountBands()
    {
        var sut = new CoverService(new FakeCoverRepository(), new FakeAuditRepository());
        var premium = sut.ComputePremium(new DateTime(2026, 1, 1), new DateTime(2026, 7, 20), CoverType.Yacht); // 200 days

        // 200 days yacht:
        // 30 * 1250*1.1 + 150 * 1250*1.1*0.95 + 20 * 1250*1.1*0.92 = 262487.5
        Assert.Equal(262487.5m, premium);
    }

    [Fact]
    public void PremiumForBulkCarrierUsesBaseRateForShortPeriod()
    {
        var sut = new CoverService(new FakeCoverRepository(), new FakeAuditRepository());
        var premium = sut.ComputePremium(new DateTime(2026, 1, 1), new DateTime(2026, 1, 11), CoverType.BulkCarrier); // 10 days

        Assert.Equal(16250m, premium); // 10 * (1250 * 1.3)
    }

    [Fact]
    public void ComputePremiumRejectsEndDateBeforeStartDate()
    {
        var sut = new CoverService(new FakeCoverRepository(), new FakeAuditRepository());

        Assert.Throws<ValidationException>(() =>
            sut.ComputePremium(new DateTime(2026, 2, 10), new DateTime(2026, 2, 10), CoverType.Yacht));
    }
}
