using Claims.Contracts;
using Claims.Entities;

namespace Claims.Mappings;

public static class CoverMappings
{
    public static CoverResponse ToResponse(this Cover cover)
    {
        return new CoverResponse
        {
            Id = cover.Id,
            StartDate = cover.StartDate,
            EndDate = cover.EndDate,
            Type = cover.Type,
            Premium = cover.Premium
        };
    }
}
