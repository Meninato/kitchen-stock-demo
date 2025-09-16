using KitchenStock.Application.Modules.Recipe.Dtos;
using KitchenStock.Application.Modules.Recipe.MediatR.Commands;
using KitchenStock.Application.Modules.Recipe.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace KitchenStock.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RecipesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public RecipesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all recipes for a kitchen
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetRecipes([FromQuery] Guid kitchenId)
    {
        var query = new GetKitchenRecipesQuery(kitchenId, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Get recipe by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipe(Guid id)
    {
        var query = new GetRecipeByIdQuery(id, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Create new recipe
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDto request)
    {
        var command = new CreateRecipeCommand(CurrentUserId, request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return ApiCreatedAtAction(
                nameof(GetRecipe),
                "Recipes",
                new { id = result.Value.Id },
                result.Value
            );
        }

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Update recipe
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] UpdateRecipeDto request)
    {
        var command = new UpdateRecipeCommand(id, CurrentUserId, request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Delete recipe
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(Guid id)
    {
        var command = new DeleteRecipeCommand(id, CurrentUserId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return NoContent();

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Calculate production capacity for a recipe
    /// </summary>
    [HttpGet("{id}/production-capacity")]
    public async Task<IActionResult> GetProductionCapacity(Guid id)
    {
        var query = new CalculateProductionCapacityQuery(id, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Produce recipe (consume ingredients from stock)
    /// </summary>
    [HttpPost("{id}/produce")]
    public async Task<IActionResult> ProduceRecipe(Guid id, [FromBody] ProduceRecipeDto request)
    {
        var command = new ProduceRecipeCommand(id, CurrentUserId, request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Get detailed cost analysis for a recipe
    /// </summary>
    [HttpGet("{id}/cost-analysis")]
    public async Task<IActionResult> GetCostAnalysis(Guid id)
    {
        var query = new GetCostAnalysisQuery(id, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }
}
