using Claims.Application.Exceptions;
using Claims.Common.Constants;
using Claims.Entities;
using Claims.Application.Contracts;
using Claims.Application.Mappings;
using Claims.Application.Models;
using Claims.Application.Repositories.Interfaces;
using Claims.Application.Services.Interfaces;
using Claims.Application.Services.Pricing;
using Claims.Enums;

namespace Claims.Application.Services;

public class CoverService : ICoverService
{
    private readonly ICoverRepository _coverRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IPremiumCalculator _premiumCalculator;

    public CoverService(
        ICoverRepository coverRepository,
        IAuditRepository auditRepository,
        IPremiumCalculator premiumCalculator)
    {
        _coverRepository = coverRepository;
        _auditRepository = auditRepository;
        _premiumCalculator = premiumCalculator;
    }

    public async Task<IReadOnlyCollection<CoverModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        var covers = await _coverRepository.GetAllAsync(cancellationToken);
        return covers.Select(x => x.ToModel()).ToArray();
    }

    public async Task<CoverModel> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var cover = await _coverRepository.GetByIdAsync(id, cancellationToken);
        if (cover is null)
        {
            throw new NotFoundException($"Cover '{id}' was not found.");
        }

        return cover.ToModel();
    }

    public async Task<CoverModel> CreateAsync(CreateCoverRequest request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var premium = ComputePremium(request.StartDate, request.EndDate, request.Type);
        var cover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = request.StartDate.Date,
            EndDate = request.EndDate.Date,
            Type = request.Type,
            Premium = premium
        };

        await _coverRepository.CreateAsync(cover, cancellationToken);
        _auditRepository.SaveCoverAudit(cover.Id, AuditConstants.HttpPost);

        return cover.ToModel();
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var deleted = await _coverRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException($"Cover '{id}' was not found.");
        }

        _auditRepository.SaveCoverAudit(id, AuditConstants.HttpDelete);
    }

    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return _premiumCalculator.Compute(startDate, endDate, coverType);
    }

    private static void ValidateRequest(CreateCoverRequest request)
    {
        var today = DateTime.UtcNow.Date;

        if (request.StartDate.Date < today)
        {
            throw new ValidationException("StartDate cannot be in the past.");
        }

        if (request.EndDate.Date <= request.StartDate.Date)
        {
            throw new ValidationException("EndDate must be greater than StartDate.");
        }

        var period = request.EndDate.Date - request.StartDate.Date;
        if (period.TotalDays > 365)
        {
            throw new ValidationException("Total insurance period cannot exceed 1 year.");
        }
    }
}

