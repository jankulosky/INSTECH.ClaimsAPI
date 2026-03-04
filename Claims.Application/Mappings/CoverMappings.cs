using Claims.Entities;
using Claims.Application.Models;

namespace Claims.Application.Mappings;

public static class CoverMappings
{
    public static CoverModel ToModel(this Cover cover)
    {
        return new CoverModel
        {
            Id = cover.Id,
            StartDate = cover.StartDate,
            EndDate = cover.EndDate,
            Type = cover.Type,
            Premium = cover.Premium
        };
    }
}

