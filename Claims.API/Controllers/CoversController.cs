using Claims.Application.Contracts;
using Claims.Application.Models;
using Claims.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICoverService _coverService;

    public CoversController(ICoverService coverService)
    {
        _coverService = coverService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CoverModel>>> GetAsync(CancellationToken cancellationToken)
    {
        return Ok(await _coverService.GetAllAsync(cancellationToken));
    }

    [ActionName(nameof(GetByIdAsync))]
    [HttpGet("{id}")]
    public async Task<ActionResult<CoverModel>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return Ok(await _coverService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost("compute")]
    public ActionResult<decimal> ComputePremium([FromBody] ComputePremiumRequest request)
    {
        return Ok(_coverService.ComputePremium(request.StartDate, request.EndDate, request.CoverType));
    }

    [HttpPost]
    public async Task<ActionResult<CoverModel>> CreateAsync([FromBody] CreateCoverRequest request, CancellationToken cancellationToken)
    {
        var created = await _coverService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _coverService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

