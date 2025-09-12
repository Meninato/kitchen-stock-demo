using KitchenStock.Application.Modules.UnitOfMeasure.Dtos;
using KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Commands;
using KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Queries;
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

    [HttpGet]
    public async Task<IActionResult> GetUnitsOfMeasure()
    {
        var query = new GetAllUnitOfMeasureQuery();
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUnitOfMeasure(Guid id)
    {
        var query = new GetUnitOfMeasureByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUnitOfMeasure([FromBody] CreateUnitOfMeasureDto request)
    {
        var command = new CreateUnitOfMeasureCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetUnitOfMeasure),
                "UnitsOfMeasure",
                new { id = result.Value.Id },
                result.Value
            );
        }

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUnitOfMeasure(Guid id, [FromBody] UpdateUnitOfMeasureDto request)
    {
        var command = new UpdateUnitOfMeasureCommand(id, request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUnitOfMeasure(Guid id)
    {
        var command = new DeleteUnitOfMeasureCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return NoContent();

        return FirstErrorToActionResult(result.Errors);
    }
}
