using Claims.Application.Contracts;
using Claims.Application.Models;
using Claims.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers;

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
    public async Task<ActionResult<IReadOnlyCollection<ClaimModel>>> GetAsync(CancellationToken cancellationToken)
    {
        return Ok(await _claimService.GetAllAsync(cancellationToken));
    }

    [ActionName(nameof(GetByIdAsync))]
    [HttpGet("{id}")]
    public async Task<ActionResult<ClaimModel>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return Ok(await _claimService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ClaimModel>> CreateAsync([FromBody] CreateClaimRequest request, CancellationToken cancellationToken)
    {
        var created = await _claimService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _claimService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

