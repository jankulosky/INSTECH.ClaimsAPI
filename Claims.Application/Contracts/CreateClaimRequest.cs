using Claims.Enums;

namespace Claims.Application.Contracts;

/// <summary>
/// Request payload for creating a claim.
/// </summary>
public class CreateClaimRequest
{
    public required string CoverId { get; set; }
    public required string Name { get; set; }
    public ClaimType Type { get; set; }
    public decimal DamageCost { get; set; }
    public DateTime Created { get; set; }
}

