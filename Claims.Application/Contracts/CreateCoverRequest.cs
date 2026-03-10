using Claims.Enums;

namespace Claims.Application.Contracts;

/// <summary>
/// Request payload for creating a cover.
/// </summary>
public class CreateCoverRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CoverType Type { get; set; }
}

