using KitchenStock.Application.Modules.Supplier.Dtos;
using KitchenStock.Application.Modules.Supplier.MediatR.Commands;
using KitchenStock.Application.Modules.Supplier.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenStock.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all suppliers for a kitchen
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSuppliers([FromQuery] Guid kitchenId)
    {
        var query = new GetKitchenSuppliersQuery(kitchenId, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Search suppliers by name
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchSuppliers([FromQuery] string searchTerm, [FromQuery] Guid kitchenId)
    {
        var query = new SearchSuppliersQuery(searchTerm, kitchenId, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Get supplier by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplier(Guid id)
    {
        var query = new GetSupplierByIdQuery(id, CurrentUserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Create new supplier
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto request)
    {
        var command = new CreateSupplierCommand(CurrentUserId, request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return ApiCreatedAtAction(
                nameof(GetSupplier),
                "Suppliers",
                new { id = result.Value.Id },
                result.Value
            );
        }

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Update supplier
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplier(Guid id, [FromBody] UpdateSupplierDto request)
    {
        var command = new UpdateSupplierCommand(id, CurrentUserId, request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Delete supplier
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(Guid id)
    {
        var command = new DeleteSupplierCommand(id, CurrentUserId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return NoContent();

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Get supplier performance analytics
    /// </summary>
    [HttpGet("{id}/performance")]
    public async Task<IActionResult> GetSupplierPerformance(Guid id, [FromQuery] DateTime? fromDate = null)
    {
        var query = new GetSupplierPerformanceQuery(id, CurrentUserId, fromDate);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }
}