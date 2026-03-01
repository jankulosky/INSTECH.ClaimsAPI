using Claims.Contracts;
using Claims.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimService _claimService;

    public ClaimsController(IClaimService claimService)
    {
        _claimService = claimService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ClaimResponse>>> GetAsync(CancellationToken cancellationToken)
    {
        return Ok(await _claimService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClaimResponse>> GetAsync(string id, CancellationToken cancellationToken)
    {
        return Ok(await _claimService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ClaimResponse>> CreateAsync([FromBody] CreateClaimRequest request, CancellationToken cancellationToken)
    {
        var created = await _claimService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAsync), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _claimService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
