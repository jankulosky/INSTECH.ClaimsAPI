using Claims.Entities;
using Claims.Data;
using Claims.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Claims.Repositories;

public class CoverRepository : ICoverRepository
{
    private readonly ClaimsDbContext _context;

    public CoverRepository(ClaimsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Cover>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Covers.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Cover?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Covers.AsNoTracking().SingleOrDefaultAsync(cover => cover.Id == id, cancellationToken);
    }

    public async Task CreateAsync(Cover cover, CancellationToken cancellationToken)
    {
        _context.Covers.Add(cover);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var cover = await _context.Covers.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (cover is null)
        {
            return false;
        }

        _context.Covers.Remove(cover);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
