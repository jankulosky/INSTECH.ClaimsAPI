using Claims.Enums;

namespace Claims.Application.Models;

public sealed class CoverModel
{
    public required string Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CoverType Type { get; set; }
    public decimal Premium { get; set; }
}
