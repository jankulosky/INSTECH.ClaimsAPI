using Claims.Enums;

namespace Claims.Application.Contracts;

public class CreateClaimRequest
{
    public required string CoverId { get; set; }
    public required string Name { get; set; }
    public ClaimType Type { get; set; }
    public decimal DamageCost { get; set; }
    public DateTime Created { get; set; }
}

