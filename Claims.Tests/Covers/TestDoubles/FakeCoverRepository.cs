using Claims.Entities;
using Claims.Application.Repositories.Interfaces;

namespace Claims.Tests;

internal sealed class FakeCoverRepository : ICoverRepository
{
    private readonly List<Cover> _covers;

    public FakeCoverRepository(params Cover[] covers)
    {
        _covers = [.. covers];
    }

    public Task<IReadOnlyCollection<Cover>> GetAllAsync(CancellationToken cancellationToken) =>
        Task.FromResult((IReadOnlyCollection<Cover>)_covers);

    public Task<Cover?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        Task.FromResult(_covers.SingleOrDefault(c => c.Id == id));

    public Task CreateAsync(Cover cover, CancellationToken cancellationToken)
    {
        _covers.Add(cover);
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var cover = _covers.SingleOrDefault(c => c.Id == id);
        if (cover is null)
        {
            return Task.FromResult(false);
        }

        _covers.Remove(cover);
        return Task.FromResult(true);
    }
}

