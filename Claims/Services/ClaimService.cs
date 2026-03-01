using Claims.Exceptions;
using Claims.Common.Constants;
using Claims.Entities;
using Claims.Contracts;
using Claims.Mappings;
using Claims.Repositories.Interfaces;
using Claims.Services.Interfaces;

namespace Claims.Services;

public class ClaimService : IClaimService
{
    private readonly IClaimRepository _claimRepository;
    private readonly ICoverRepository _coverRepository;
    private readonly IAuditRepository _auditRepository;

    public ClaimService(IClaimRepository claimRepository, ICoverRepository coverRepository, IAuditRepository auditRepository)
    {
        _claimRepository = claimRepository;
        _coverRepository = coverRepository;
        _auditRepository = auditRepository;
    }

    public async Task<IReadOnlyCollection<ClaimResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var claims = await _claimRepository.GetAllAsync(cancellationToken);
        return claims.Select(x => x.ToResponse()).ToArray();
    }

    public async Task<ClaimResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var claim = await _claimRepository.GetByIdAsync(id, cancellationToken);
        if (claim is null)
        {
            throw new NotFoundException($"Claim '{id}' was not found.");
        }

        return claim.ToResponse();
    }

    public async Task<ClaimResponse> CreateAsync(CreateClaimRequest request, CancellationToken cancellationToken)
    {
        await ValidateRequestAsync(request, cancellationToken);

        var claim = new Claim
        {
            Id = Guid.NewGuid().ToString(),
            CoverId = request.CoverId,
            Name = request.Name,
            Type = request.Type,
            DamageCost = request.DamageCost,
            Created = request.Created.Date
        };

        await _claimRepository.CreateAsync(claim, cancellationToken);
        await _auditRepository.SaveClaimAuditAsync(claim.Id, AuditConstants.HttpPost, cancellationToken);

        return claim.ToResponse();
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var deleted = await _claimRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException($"Claim '{id}' was not found.");
        }

        await _auditRepository.SaveClaimAuditAsync(id, AuditConstants.HttpDelete, cancellationToken);
    }

    private async Task ValidateRequestAsync(CreateClaimRequest request, CancellationToken cancellationToken)
    {
        if (request.DamageCost > 100000m)
        {
            throw new ValidationException("DamageCost cannot exceed 100000.");
        }

        if (string.IsNullOrWhiteSpace(request.CoverId))
        {
            throw new ValidationException("CoverId is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("Name is required.");
        }

        var cover = await _coverRepository.GetByIdAsync(request.CoverId, cancellationToken);
        if (cover is null)
        {
            throw new ValidationException($"Cover '{request.CoverId}' does not exist.");
        }

        var createdDate = request.Created.Date;
        if (createdDate < cover.StartDate.Date || createdDate > cover.EndDate.Date)
        {
            throw new ValidationException("Claim Created date must be within the related Cover period.");
        }
    }
}
