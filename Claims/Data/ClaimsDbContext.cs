using Claims.Common.Constants;
using Claims.Entities;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data;

public class ClaimsDbContext : DbContext
{
    public ClaimsDbContext(DbContextOptions<ClaimsDbContext> options) : base(options)
    {
    }

    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<Cover> Covers => Set<Cover>();
    public DbSet<ClaimAudit> ClaimAudits => Set<ClaimAudit>();
    public DbSet<CoverAudit> CoverAudits => Set<CoverAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Claim>().ToTable(DataConstants.ClaimsTableName);
        modelBuilder.Entity<Cover>().ToTable(DataConstants.CoversTableName);
        modelBuilder.Entity<ClaimAudit>().ToTable(DataConstants.ClaimAuditsTableName);
        modelBuilder.Entity<CoverAudit>().ToTable(DataConstants.CoverAuditsTableName);

        modelBuilder.Entity<Claim>().Property(x => x.DamageCost).HasPrecision(18, 2);
        modelBuilder.Entity<Cover>().Property(x => x.Premium).HasPrecision(18, 2);
    }
}
