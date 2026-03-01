using Claims.Exceptions;
using Claims.Entities;
using Claims.Enums;
using Claims.Contracts;
using Claims.Services;
using Xunit;

namespace Claims.Tests;

public class CoverServiceTests
{
    [Fact]
    public async Task CreateCoverRejectsStartDateInThePast()
    {
        var service = new CoverService(new FakeCoverRepository(), new FakeAuditRepository());

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateCoverRequest
        {
            StartDate = DateTime.UtcNow.Date.AddDays(-1),
            EndDate = DateTime.UtcNow.Date.AddDays(5),
            Type = CoverType.ContainerShip
        }, CancellationToken.None));
    }

    [Fact]
    public async Task CreateCoverRejectsInsurancePeriodLongerThanOneYear()
    {
        var service = new CoverService(new FakeCoverRepository(), new FakeAuditRepository());

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateCoverRequest
        {
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            EndDate = DateTime.UtcNow.Date.AddDays(367),
            Type = CoverType.ContainerShip
        }, CancellationToken.None));
    }

    [Fact]
    public async Task GetCoverByIdThrowsWhenCoverDoesNotExist()
    {
        var service = new CoverService(new FakeCoverRepository(), new FakeAuditRepository());

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync("missing-cover", CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCoverThrowsWhenCoverDoesNotExist()
    {
        var service = new CoverService(new FakeCoverRepository(), new FakeAuditRepository());

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync("missing-cover", CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCoverWritesDeleteAuditEntry()
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
        var service = new CoverService(coverRepository, auditRepository);

        await service.DeleteAsync("cover-1", CancellationToken.None);

        Assert.Single(auditRepository.CoverAudits);
        Assert.Equal("DELETE", auditRepository.CoverAudits[0].Verb);
    }
}
