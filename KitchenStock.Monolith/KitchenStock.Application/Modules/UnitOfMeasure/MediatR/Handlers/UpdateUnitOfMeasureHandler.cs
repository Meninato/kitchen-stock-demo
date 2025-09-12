using KitchenStock.Application.Modules.UnitOfMeasure.Abstractions;
using KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Commands;
using KitchenStock.Application.Modules.UnitOfMeasure.Results;
using MediatR;

namespace KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Handlers;

public class UpdateUnitOfMeasureHandler : IRequestHandler<UpdateUnitOfMeasureCommand, UnitOfMeasureResult>
{
    private readonly IUnitOfMeasureService _unitOfMeasureService;

    public UpdateUnitOfMeasureHandler(IUnitOfMeasureService unitOfMeasureService)
    {
        _unitOfMeasureService = unitOfMeasureService;
    }

    public async Task<UnitOfMeasureResult> Handle(UpdateUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfMeasureService.UpdateUnitAsync(request.UomId, request.Dto);
    }
}