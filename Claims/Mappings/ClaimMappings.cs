using Claims.Contracts;
using Claims.Entities;

namespace Claims.Mappings;

public static class ClaimMappings
{
    public static ClaimResponse ToResponse(this Claim claim)
    {
        return new ClaimResponse
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
