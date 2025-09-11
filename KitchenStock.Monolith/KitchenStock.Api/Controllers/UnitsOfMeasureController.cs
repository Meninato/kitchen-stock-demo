using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenStock.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UnitsOfMeasureController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public UnitsOfMeasureController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all available units of measure
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetUnitsOfMeasure()
    {
        var result = await _unitService.GetAllUnitsAsync();

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.FirstToActionResult(this);
    }

    /// <summary>
    /// Get unit of measure by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 404)]
    public async Task<IActionResult> GetUnitOfMeasure(int id)
    {
        var result = await _unitService.GetUnitByIdAsync(id);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.FirstToActionResult(this);
    }

    /// <summary>
    /// Create new unit of measure (admin only)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> CreateUnitOfMeasure([FromBody] CreateUnitOfMeasureRequest request)
    {
        var result = await _unitService.CreateUnitAsync(request);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetUnitOfMeasure),
                new { id = result.Value.Id },
                new { success = true, data = result.Value }
            );
        }

        return result.FirstToActionResult(this);
    }

    /// <summary>
    /// Update unit of measure
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    public async Task<IActionResult> UpdateUnitOfMeasure(int id, [FromBody] UpdateUnitOfMeasureRequest request)
    {
        var result = await _unitService.UpdateUnitAsync(id, request);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.FirstToActionResult(this);
    }

    /// <summary>
    /// Delete unit of measure
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    public async Task<IActionResult> DeleteUnitOfMeasure(int id)
    {
        var result = await _unitService.DeleteUnitAsync(id);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return result.FirstToActionResult(this);
    }
}
