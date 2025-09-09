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
    [ProducesResponseType(typeof(List<IngredientResponse>), 200)]
    public async Task<IActionResult> GetIngredients([FromQuery] int kitchenId)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.GetKitchenIngredientsAsync(kitchenId, userId);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Data });
        }

        return BadRequest(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Get low stock ingredients for a kitchen
    /// </summary>
    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(List<IngredientResponse>), 200)]
    public async Task<IActionResult> GetLowStockIngredients([FromQuery] int kitchenId)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.GetLowStockIngredientsAsync(kitchenId, userId);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Data });
        }

        return BadRequest(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Get ingredient by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IngredientResponse), 200)]
    [ProducesResponseType(typeof(object), 404)]
    public async Task<IActionResult> GetIngredient(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.GetIngredientByIdAsync(id, userId);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Data });
        }

        return NotFound(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Create new ingredient
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(IngredientResponse), 201)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.CreateIngredientAsync(userId, request);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetIngredient),
                new { id = result.Data!.Id },
                new { success = true, data = result.Data }
            );
        }

        return BadRequest(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Update ingredient
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(IngredientResponse), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    public async Task<IActionResult> UpdateIngredient(int id, [FromBody] UpdateIngredientRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.UpdateIngredientAsync(id, userId, request);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Data });
        }

        return BadRequest(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Update ingredient stock
    /// </summary>
    [HttpPatch("{id}/stock")]
    [ProducesResponseType(typeof(IngredientResponse), 200)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _ingredientService.UpdateStockAsync(id, userId, request);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Data });
        }

        return BadRequest(new { success = false, message = result.ErrorMessage });
    }
}
