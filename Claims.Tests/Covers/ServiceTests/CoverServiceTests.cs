using Claims.Application.Exceptions;
using Claims.Entities;
using Claims.Enums;
using Claims.Application.Contracts;
using Claims.Application.Services;
using Claims.Application.Services.Pricing;
using Xunit;

namespace Claims.Tests;

public class CoverServiceTests
{
    private static CoverService CreateService(FakeCoverRepository? coverRepository = null, FakeAuditRepository? auditRepository = null)
    {
        return new CoverService(
            coverRepository ?? new FakeCoverRepository(),
            auditRepository ?? new FakeAuditRepository(),
            new PremiumCalculator(
            [
                new YachtPremiumProfile(),
                new PassengerShipPremiumProfile(),
                new TankerPremiumProfile(),
                new DefaultPremiumProfile()
            ]));
    }

    [Fact]
    public async Task CreateCoverFailsWhenStartDateIsInThePast()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateCoverRequest
        {
            StartDate = DateTime.UtcNow.Date.AddDays(-1),
            EndDate = DateTime.UtcNow.Date.AddDays(5),
            Type = CoverType.ContainerShip
        }, CancellationToken.None));
    }

    [Fact]
    public async Task CreateCoverFailsWhenInsurancePeriodIsLongerThanOneYear()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateCoverRequest
        {
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            EndDate = DateTime.UtcNow.Date.AddDays(367),
            Type = CoverType.ContainerShip
        }, CancellationToken.None));
    }

    [Fact]
    public async Task GetCoverByIdThrowsWhenCoverIsMissing()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync("missing-cover", CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCoverThrowsWhenCoverIsMissing()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync("missing-cover", CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCoverWritesDeleteAuditRecord()
    {
        var coverRepository = new FakeCoverRepository(new Cover
        {
            Id = "cover-1",
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            EndDate = DateTime.UtcNow.Date.AddDays(10),
            Type = CoverType.BulkCarrier,
            Premium = 1000m
        });
        var auditRepository = new FakeAuditRepository();
        var service = CreateService(coverRepository, auditRepository);

        await service.DeleteAsync("cover-1", CancellationToken.None);

        Assert.Single(auditRepository.CoverAudits);
        Assert.Equal("DELETE", auditRepository.CoverAudits[0].Verb);
    }
}

