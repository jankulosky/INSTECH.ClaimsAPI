using Claims.Enums;

namespace Claims.Entities;

public class Cover
{
    public required string Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public CoverType Type { get; set; }

    public decimal Premium { get; set; }
}
