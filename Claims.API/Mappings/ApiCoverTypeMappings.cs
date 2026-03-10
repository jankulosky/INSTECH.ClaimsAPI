using Claims.API.Contracts;
using Claims.Enums;

namespace Claims.API.Mappings;

public static class ApiCoverTypeMappings
{
    public static CoverType ToDomain(this ApiCoverType coverType)
    {
        return coverType switch
        {
            ApiCoverType.Yacht => CoverType.Yacht,
            ApiCoverType.PassengerShip => CoverType.PassengerShip,
            ApiCoverType.Tanker => CoverType.Tanker,
            ApiCoverType.ContainerShip => CoverType.ContainerShip,
            ApiCoverType.BulkCarrier => CoverType.BulkCarrier,
            _ => throw new ArgumentOutOfRangeException(nameof(coverType), coverType, "Unsupported cover type.")
        };
    }
}
