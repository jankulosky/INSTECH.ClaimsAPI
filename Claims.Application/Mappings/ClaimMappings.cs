using Claims.Entities;
using Claims.Application.Models;

namespace Claims.Application.Mappings;

public static class ClaimMappings
{
    public static ClaimModel ToModel(this Claim claim)
    {
        return new ClaimModel
        {
            Id = claim.Id,
            CoverId = claim.CoverId,
            Name = claim.Name,
            Type = claim.Type,
            DamageCost = claim.DamageCost,
            Created = claim.Created
        };
    }
}

