using KitchenStock.Application.Modules.UnitOfMeasure.Abstractions;
using KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Commands;
using KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Queries;
using KitchenStock.Application.Modules.UnitOfMeasure.Results;
using MediatR;

namespace KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Handlers;

public class CreateUnitOfMeasureHandler : IRequestHandler<CreateUnitOfMeasureCommand, UnitOfMeasureResult>
{
    private readonly IUnitOfMeasureService _unitOfMeasureService;

    public CreateUnitOfMeasureHandler(IUnitOfMeasureService unitOfMeasureService)
    {
        _unitOfMeasureService = unitOfMeasureService;
    }

    public async Task<UnitOfMeasureResult> Handle(CreateUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfMeasureService.CreateUnitAsync(request.Dto);
    }
}