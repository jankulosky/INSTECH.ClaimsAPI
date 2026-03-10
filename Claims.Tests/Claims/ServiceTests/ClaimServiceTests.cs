using Claims.Application.Exceptions;
using Claims.Entities;
using Claims.Enums;
using Claims.Application.Contracts;
using Claims.Application.Services;
using Xunit;

namespace Claims.Tests;

public class ClaimServiceTests
{
    [Fact]
    public async Task CreateClaimFailsWhenDamageCostExceedsAllowedLimit()
    {
        var claimRepository = new FakeClaimRepository();
        var coverRepository = new FakeCoverRepository(new Cover
        {
            Id = "cover-1",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(10),
            Type = CoverType.Yacht,
            Premium = 1000
        });
        var auditRepository = new FakeAuditRepository();

        var service = new ClaimService(claimRepository, coverRepository, auditRepository);

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateClaimRequest
        {
            CoverId = "cover-1",
            Name = "high claim",
            Type = ClaimType.Fire,
            Created = DateTime.UtcNow.Date,
            DamageCost = 100001m
        }, CancellationToken.None));
    }

    [Fact]
    public async Task CreateClaimFailsWhenCoverIdIsMissing()
    {
        var service = new ClaimService(new FakeClaimRepository(), new FakeCoverRepository(), new FakeAuditRepository());

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateClaimRequest
        {
            CoverId = "",
            Name = "missing cover",
            Type = ClaimType.Fire,
            Created = DateTime.UtcNow.Date,
            DamageCost = 100m
        }, CancellationToken.None));
    }

    [Fact]
    public async Task CreateClaimFailsWhenNameIsMissing()
    {
        var service = new ClaimService(new FakeClaimRepository(), new FakeCoverRepository(new Cover
        {
            Id = "cover-1",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(10),
            Type = CoverType.Yacht,
            Premium = 500m
        }), new FakeAuditRepository());

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateClaimRequest
        {
            CoverId = "cover-1",
            Name = " ",
            Type = ClaimType.Fire,
            Created = DateTime.UtcNow.Date,
            DamageCost = 100m
        }, CancellationToken.None));
    }

    [Fact]
    public async Task CreateClaimFailsWhenCreatedDateIsOutsideCoverPeriod()
    {
        var claimRepository = new FakeClaimRepository();
        var coverRepository = new FakeCoverRepository(new Cover
        {
            Id = "cover-1",
            StartDate = new DateTime(2026, 2, 1),
            EndDate = new DateTime(2026, 2, 28),
            Type = CoverType.Yacht,
            Premium = 1000
        });
        var auditRepository = new FakeAuditRepository();

        var service = new ClaimService(claimRepository, coverRepository, auditRepository);

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateClaimRequest
        {
            CoverId = "cover-1",
            Name = "bad date claim",
            Type = ClaimType.Collision,
            Created = new DateTime(2026, 3, 1),
            DamageCost = 500m
        }, CancellationToken.None));
    }

    [Fact]
    public async Task CreateClaimWritesPostAuditRecord()
    {
        var claimRepository = new FakeClaimRepository();
        var coverRepository = new FakeCoverRepository(new Cover
        {
            Id = "cover-1",
            StartDate = new DateTime(2026, 2, 1),
            EndDate = new DateTime(2026, 2, 28),
            Type = CoverType.Yacht,
            Premium = 1000
        });
        var auditRepository = new FakeAuditRepository();

        var service = new ClaimService(claimRepository, coverRepository, auditRepository);

        await service.CreateAsync(new CreateClaimRequest
        {
            CoverId = "cover-1",
            Name = "valid claim",
            Type = ClaimType.Collision,
            Created = new DateTime(2026, 2, 10),
            DamageCost = 5000m
        }, CancellationToken.None);

        Assert.Single(auditRepository.ClaimAudits);
        Assert.Equal("POST", auditRepository.ClaimAudits[0].Verb);
    }

    [Fact]
    public async Task CreateClaimAllowsDamageCostAtExactMaximum()
    {
        var claimRepository = new FakeClaimRepository();
        var coverRepository = new FakeCoverRepository(new Cover
        {
            Id = "cover-1",
            StartDate = new DateTime(2026, 2, 1),
            EndDate = new DateTime(2026, 2, 28),
            Type = CoverType.Yacht,
            Premium = 1000
        });
        var service = new ClaimService(claimRepository, coverRepository, new FakeAuditRepository());

        var created = await service.CreateAsync(new CreateClaimRequest
        {
            CoverId = "cover-1",
            Name = "boundary claim",
            Type = ClaimType.Collision,
            Created = new DateTime(2026, 2, 10),
            DamageCost = 100000m
        }, CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(created.Id));
        Assert.Equal(100000m, created.DamageCost);
    }

    [Fact]
    public async Task GetClaimByIdThrowsWhenClaimIsMissing()
    {
        var service = new ClaimService(new FakeClaimRepository(), new FakeCoverRepository(), new FakeAuditRepository());

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync("missing-claim", CancellationToken.None));
    }

    [Fact]
    public async Task DeleteClaimThrowsWhenClaimIsMissing()
    {
        var service = new ClaimService(new FakeClaimRepository(), new FakeCoverRepository(), new FakeAuditRepository());

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync("missing-claim", CancellationToken.None));
    }

    [Fact]
    public async Task DeleteClaimWritesDeleteAuditRecord()
    {
        var claimRepository = new FakeClaimRepository();
        await claimRepository.CreateAsync(new Claim
        {
            Id = "claim-1",
            CoverId = "cover-1",
            Name = "test",
            Type = ClaimType.Fire,
            DamageCost = 1200m,
            Created = new DateTime(2026, 2, 10)
        }, CancellationToken.None);

        var auditRepository = new FakeAuditRepository();
        var service = new ClaimService(claimRepository, new FakeCoverRepository(), auditRepository);

        await service.DeleteAsync("claim-1", CancellationToken.None);

        Assert.Single(auditRepository.ClaimAudits);
        Assert.Equal("DELETE", auditRepository.ClaimAudits[0].Verb);
    }
}

