using Claims.Entities;
using Claims.Data;
using Claims.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Claims.Repositories;

public class ClaimRepository : IClaimRepository
{
    private readonly ClaimsDbContext _context;

    public ClaimRepository(ClaimsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Claim>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Claims.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Claim?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Claims.AsNoTracking().SingleOrDefaultAsync(claim => claim.Id == id, cancellationToken);
    }

    public async Task CreateAsync(Claim claim, CancellationToken cancellationToken)
    {
        _context.Claims.Add(claim);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var claim = await _context.Claims.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (claim is null)
        {
            return false;
        }

        _context.Claims.Remove(claim);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
