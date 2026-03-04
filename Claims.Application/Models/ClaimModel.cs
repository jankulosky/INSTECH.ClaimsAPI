using Claims.Enums;

namespace Claims.Application.Models;

public sealed class ClaimModel
{
    public required string Id { get; set; }
    public required string CoverId { get; set; }
    public required string Name { get; set; }
    public ClaimType Type { get; set; }
    public decimal DamageCost { get; set; }
    public DateTime Created { get; set; }
}
