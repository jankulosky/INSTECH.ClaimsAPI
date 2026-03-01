using Claims.Exceptions;
using Claims.Common.Constants;
using Claims.Entities;
using Claims.Contracts;
using Claims.Enums;
using Claims.Mappings;
using Claims.Repositories.Interfaces;
using Claims.Services.Interfaces;

namespace Claims.Services;

public class CoverService : ICoverService
{
    private readonly ICoverRepository _coverRepository;
    private readonly IAuditRepository _auditRepository;

    public CoverService(
        ICoverRepository coverRepository,
        IAuditRepository auditRepository)
    {
        _coverRepository = coverRepository;
        _auditRepository = auditRepository;
    }

    public async Task<IReadOnlyCollection<CoverResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var covers = await _coverRepository.GetAllAsync(cancellationToken);
        return covers.Select(x => x.ToResponse()).ToArray();
    }

    public async Task<CoverResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var cover = await _coverRepository.GetByIdAsync(id, cancellationToken);
        if (cover is null)
        {
            throw new NotFoundException($"Cover '{id}' was not found.");
        }

        return cover.ToResponse();
    }

    public async Task<CoverResponse> CreateAsync(CreateCoverRequest request, CancellationToken cancellationToken)
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
        await _auditRepository.SaveCoverAuditAsync(cover.Id, AuditConstants.HttpPost, cancellationToken);

        return cover.ToResponse();
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var deleted = await _coverRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException($"Cover '{id}' was not found.");
        }

        await _auditRepository.SaveCoverAuditAsync(id, AuditConstants.HttpDelete, cancellationToken);
    }

    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        if (endDate <= startDate)
        {
            throw new ValidationException("EndDate must be greater than StartDate.");
        }

        const decimal baseDayRate = 1250m;
        var totalDays = (int)(endDate.Date - startDate.Date).TotalDays;
        var dailyBaseRate = baseDayRate * GetTypeMultiplier(coverType);

        var firstBandDays = Math.Min(30, totalDays);
        var secondBandDays = Math.Min(150, Math.Max(0, totalDays - 30));
        var thirdBandDays = Math.Max(0, totalDays - 180);

        var secondBandDiscount = coverType == CoverType.Yacht ? 0.05m : 0.02m;
        var thirdBandDiscount = coverType == CoverType.Yacht ? 0.08m : 0.03m;

        var firstBandPremium = firstBandDays * dailyBaseRate;
        var secondBandPremium = secondBandDays * dailyBaseRate * (1m - secondBandDiscount);
        var thirdBandPremium = thirdBandDays * dailyBaseRate * (1m - thirdBandDiscount);

        return firstBandPremium + secondBandPremium + thirdBandPremium;
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

    private static decimal GetTypeMultiplier(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
        };
    }
}
