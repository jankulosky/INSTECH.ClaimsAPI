using Claims.Application.Exceptions;
using Claims.Application.Services;
using Claims.Application.Services.Pricing;
using Claims.Enums;
using Xunit;

namespace Claims.Tests;

public class CoverPremiumCalculationTests
{
    private static CoverService CreateService()
    {
        return new CoverService(
            new FakeCoverRepository(),
            new FakeAuditRepository(),
            new PremiumCalculator(
            [
                new YachtPremiumProfile(),
                new PassengerShipPremiumProfile(),
                new TankerPremiumProfile(),
                new DefaultPremiumProfile()
            ]));
    }

    [Fact]
    public void ComputePremiumUsesProgressiveDiscountsForYachtAcrossAllBands()
    {
        var service = CreateService();

        var premium = service.ComputePremium(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 7, 20),
            CoverType.Yacht);

        Assert.Equal(262487.5m, premium);
    }

    [Fact]
    public void ComputePremiumAppliesOnlyBaseRateForPassengerShipWithinThirtyDays()
    {
        var service = CreateService();

        var premium = service.ComputePremium(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 31),
            CoverType.PassengerShip);

        Assert.Equal(45000m, premium);
    }

    [Fact]
    public void ComputePremiumAppliesSecondBandDiscountForTankerAtBandBoundary()
    {
        var service = CreateService();

        var premium = service.ComputePremium(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 6, 30),
            CoverType.Tanker);

        Assert.Equal(326250m, premium);
    }

    [Fact]
    public void ComputePremiumAppliesThirdBandDiscountForDefaultTypeAfterOneHundredEightyDays()
    {
        var service = CreateService();

        var premium = service.ComputePremium(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 7, 20),
            CoverType.BulkCarrier);

        Assert.Equal(315250m, premium);
    }

    [Fact]
    public void ComputePremiumUsesDefaultMultiplierForShortDefaultTypePeriod()
    {
        var service = CreateService();

        var premium = service.ComputePremium(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 11),
            CoverType.ContainerShip);

        Assert.Equal(16250m, premium);
    }

    [Fact]
    public void ComputePremiumChoosesSpecificProfileOverDefaultProfile()
    {
        var calculator = new PremiumCalculator(
        [
            new DefaultPremiumProfile(),
            new YachtPremiumProfile()
        ]);

        var premium = calculator.Compute(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 11),
            CoverType.Yacht);

        Assert.Equal(13750m, premium);
    }

    [Fact]
    public void ComputePremiumThrowsValidationExceptionWhenEndDateIsNotAfterStartDate()
    {
        var service = CreateService();

        Assert.Throws<ValidationException>(() =>
            service.ComputePremium(
                new DateTime(2026, 2, 10),
                new DateTime(2026, 2, 10),
                CoverType.Yacht));
    }
}
