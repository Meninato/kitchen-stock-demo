using KitchenStock.Application.Modules.Stock.Dtos;
using KitchenStock.Application.Modules.Stock.MediatR.Commands;
using KitchenStock.Application.Modules.Stock.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenStock.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StockController  :ApiControllerBase
{
    private readonly IMediator _mediator;

    public StockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetKitchenStockHistory(
        [FromQuery] Guid kitchenId,
        [FromQuery] DateTime? fromDate = null)
    {
        var query = new GetKitchenStockHistoryQuery(kitchenId, CurrentUserId, fromDate);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpGet("ingredients/{ingredientId}/history")]
    public async Task<IActionResult> GetIngredientStockHistory(Guid ingredientId)
    {
        var query = new GetIngredientStockHistoryQuery(ingredientId, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetStockSummary([FromQuery] Guid kitchenId)
    {
        var query = new GetStockSummaryQuery(kitchenId, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpPost("entries")]
    public async Task<IActionResult> AddStockEntry([FromBody] CreateStockEntryDto request)
    {
        var command = new AddStockEntryCommand(CurrentUserId, request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return ApiCreatedAtAction(
                nameof(GetStockEntry),
                "Stock",
                new { id = result.Value.Id },
                result.Value
            );
        }

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpGet("entries/{id}")]
    public async Task<IActionResult> GetStockEntry(Guid id)
    {
        var query = new GetStockEntryByIdQuery(id, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }
}