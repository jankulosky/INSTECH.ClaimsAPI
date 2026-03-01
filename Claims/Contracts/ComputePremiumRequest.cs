using Claims.Enums;

namespace Claims.Contracts;

public class ComputePremiumRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CoverType CoverType { get; set; }
}
