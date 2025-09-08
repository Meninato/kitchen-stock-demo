using KitchenStock.Application.Kitchen.Commands;
using KitchenStock.Application.Kitchen.Dtos;
using KitchenStock.Application.Kitchen.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenStock.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class KitchensController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public KitchensController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetKitchens()
    {
        var query = new GetUserKitchensQuery(CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetKitchen(Guid id)
    {
        var query = new GetKitchenByIdQuery(id, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> CreateKitchen([FromBody] CreateKitchenDto request)
    {
        var command = new CreateKitchenCommand(CurrentUserId, request.Name, request.Description);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return ApiCreatedAtAction(
                nameof(GetKitchen),
                "Kitchens",
                new { id = result.Value.Id },
                result.Value
            );
        }

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateKitchen(Guid id, [FromBody] UpdateKitchenDto request)
    {
        var command = new UpdateKitchenCommand(request.KitchenId, CurrentUserId, request.Name, request.Description);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return ApiOk(result);


    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteKitchen(Guid id)
    {
        var command = new DeleteKitchenCommand(id, CurrentUserId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return NoContent();

        return FirstErrorToActionResult(result.Errors);
    }
}
