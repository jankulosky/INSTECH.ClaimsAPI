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
    public async Task ComputePremiumUsesProgressiveDiscountsForYachtAcrossAllBands()
    {
        var service = CreateService();

        var premium = await service.ComputePremiumAsync(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 7, 20),
            CoverType.Yacht,
            CancellationToken.None);

        Assert.Equal(262487.5m, premium);
    }

    [Fact]
    public async Task ComputePremiumAppliesOnlyBaseRateForPassengerShipWithinThirtyDays()
    {
        var service = CreateService();

        var premium = await service.ComputePremiumAsync(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 31),
            CoverType.PassengerShip,
            CancellationToken.None);

        Assert.Equal(45000m, premium);
    }

    [Fact]
    public async Task ComputePremiumAppliesSecondBandDiscountForTankerAtBandBoundary()
    {
        var service = CreateService();

        var premium = await service.ComputePremiumAsync(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 6, 30),
            CoverType.Tanker,
            CancellationToken.None);

        Assert.Equal(326250m, premium);
    }

    [Fact]
    public async Task ComputePremiumAppliesThirdBandDiscountForDefaultTypeAfterOneHundredEightyDays()
    {
        var service = CreateService();

        var premium = await service.ComputePremiumAsync(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 7, 20),
            CoverType.BulkCarrier,
            CancellationToken.None);

        Assert.Equal(315250m, premium);
    }

    [Fact]
    public async Task ComputePremiumUsesDefaultMultiplierForShortDefaultTypePeriod()
    {
        var service = CreateService();

        var premium = await service.ComputePremiumAsync(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 11),
            CoverType.ContainerShip,
            CancellationToken.None);

        Assert.Equal(16250m, premium);
    }

    [Fact]
    public async Task ComputePremiumComputesSingleDayPeriod()
    {
        var service = CreateService();

        var premium = await service.ComputePremiumAsync(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 2),
            CoverType.BulkCarrier,
            CancellationToken.None);

        Assert.Equal(1625m, premium);
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
    public async Task ComputePremiumThrowsValidationExceptionWhenEndDateIsNotAfterStartDate()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(async () =>
            await service.ComputePremiumAsync(
                new DateTime(2026, 2, 10),
                new DateTime(2026, 2, 10),
                CoverType.Yacht,
                CancellationToken.None));
    }
}
