using Claims.Enums;

namespace Claims.Entities;

public class Claim
{
    public required string Id { get; set; }

    public required string CoverId { get; set; }

    public DateTime Created { get; set; }

    public required string Name { get; set; }

    public ClaimType Type { get; set; }

    public decimal DamageCost { get; set; }
}

