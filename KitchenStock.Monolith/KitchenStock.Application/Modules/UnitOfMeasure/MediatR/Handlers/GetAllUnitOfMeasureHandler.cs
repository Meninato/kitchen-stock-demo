using KitchenStock.Application.Modules.UnitOfMeasure.Abstractions;
using KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Queries;
using KitchenStock.Application.Modules.UnitOfMeasure.Results;
using MediatR;

namespace KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Handlers;

public class GetAllUnitOfMeasureHandler : IRequestHandler<GetAllUnitOfMeasureQuery, UnitOfMeasureListResult>
{
    private readonly IUnitOfMeasureService _unitOfMeasureService;

    public GetAllUnitOfMeasureHandler(IUnitOfMeasureService unitOfMeasureService)
    {
        _unitOfMeasureService = unitOfMeasureService;
    }

    public async Task<UnitOfMeasureListResult> Handle(GetAllUnitOfMeasureQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfMeasureService.GetAllUnitsAsync();
    }
}
