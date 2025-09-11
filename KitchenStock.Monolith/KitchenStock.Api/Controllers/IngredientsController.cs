using KitchenStock.Application.Modules.Ingredients.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenStock.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public IngredientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetIngredients([FromQuery] Guid kitchenId)
    {
        var query = new GetIngredientsQuery(kitchenId, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Get low stock ingredients for a kitchen
    /// </summary>
    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> GetLowStockIngredients([FromQuery] int kitchenId)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.GetLowStockIngredientsAsync(kitchenId, userId);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.FirstToActionResult(this);
    }

    /// <summary>
    /// Get ingredient by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 404)]
    public async Task<IActionResult> GetIngredient(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.GetIngredientByIdAsync(id, userId);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.FirstToActionResult(this);
    }

    /// <summary>
    /// Create new ingredient
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.CreateIngredientAsync(userId, request);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetIngredient),
                new { id = result.Value.Id },
                new { success = true, data = result.Value }
            );
        }

        return result.FirstToActionResult(this);
    }

    /// <summary>
    /// Update ingredient
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> UpdateIngredient(int id, [FromBody] UpdateIngredientRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.UpdateIngredientAsync(id, userId, request);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.FirstToActionResult(this);
    }

    /// <summary>
    /// Update ingredient stock
    /// </summary>
    [HttpPatch("{id}/stock")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.UpdateStockAsync(id, userId, request);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.FirstToActionResult(this);
    }

    /// <summary>
    /// Delete ingredient
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.DeleteIngredientAsync(id, userId);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return result.FirstToActionResult(this);
    }
}
