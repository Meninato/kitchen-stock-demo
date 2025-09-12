using KitchenStock.Application.Modules.UnitOfMeasure.Abstractions;
using KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Commands;
using KitchenStock.Application.Modules.UnitOfMeasure.Results;
using MediatR;

namespace KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Handlers;

public class DeleteUnitOfMeasureHandler : IRequestHandler<DeleteUnitOfMeasureCommand, UnitOfMeasureResult>
{
    private readonly IUnitOfMeasureService _unitOfMeasureService;

    public DeleteUnitOfMeasureHandler(IUnitOfMeasureService unitOfMeasureService)
    {
        _unitOfMeasureService = unitOfMeasureService;
    }

    public async Task<UnitOfMeasureResult> Handle(DeleteUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfMeasureService.DeleteUnitAsync(request.UomId);
    }
}