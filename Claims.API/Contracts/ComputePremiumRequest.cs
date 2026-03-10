namespace Claims.API.Contracts;

/// <summary>
/// API request payload for premium computation.
/// </summary>
public sealed class ComputePremiumRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ApiCoverType CoverType { get; set; }
}
