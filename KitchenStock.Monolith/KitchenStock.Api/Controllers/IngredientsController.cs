using KitchenStock.Api.Common.Pagination;
using KitchenStock.Application.Common.Pagination.Dtos;
using KitchenStock.Application.Modules.Ingredients.Dtos;
using KitchenStock.Application.Modules.Ingredients.MediatR.Commands;
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

    /// <summary>
    /// Get paginated ingredients for a kitchen with filtering and sorting
    /// </summary>
    public async Task<IActionResult> GetIngredients(
        [FromQuery] PaginationDto pagination,
        [FromQuery] Guid kitchenId,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool lowStockOnly = false)
    {
        var filter = new IngredientFilterDto(searchTerm, lowStockOnly);

        var query = new GetIngredientsPagedQuery(kitchenId, CurrentUserId, pagination, filter);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result.Value.Items, b => b.WithPagination(result.Value.Details));

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockIngredients([FromQuery] Guid kitchenId)
    {
        var query = new GetLowStockIngredientsQuery(kitchenId, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result.Value);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIngredient(Guid id)
    {
        var query = new GetIngredientQuery(id, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result.Value);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientDto request)
    {
        var command = new CreateIngredientCommand(CurrentUserId, request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return ApiCreatedAtAction(
                nameof(GetIngredient),
                "Ingredients",
                new { id = result.Value.Id },
                result.Value
            );
        }

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIngredient(Guid id, [FromBody] UpdateIngredientDto request)
    {
        var command = new UpdateIngredientCommand(id, CurrentUserId, request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return ApiOk(result.Value);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockDto request)
    {
        var command = new UpdateStockCommand(id, CurrentUserId, request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return ApiOk(result.Value);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngredient(Guid id)
    {
        var command = new DeleteIngredientCommand(id, CurrentUserId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return NoContent();

        return FirstErrorToActionResult(result.Errors);
    }
}
