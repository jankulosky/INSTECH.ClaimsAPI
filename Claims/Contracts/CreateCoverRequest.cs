using Claims.Enums;

namespace Claims.Contracts;

public class CreateCoverRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CoverType Type { get; set; }
}
