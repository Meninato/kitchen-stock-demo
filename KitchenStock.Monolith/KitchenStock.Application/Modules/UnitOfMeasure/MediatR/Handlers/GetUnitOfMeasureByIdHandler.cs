using KitchenStock.Application.Modules.UnitOfMeasure.Abstractions;
using KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Queries;
using KitchenStock.Application.Modules.UnitOfMeasure.Results;
using MediatR;

namespace KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Handlers;

public class GetUnitOfMeasureByIdHandler : IRequestHandler<GetUnitOfMeasureByIdQuery, UnitOfMeasureResult>
{
    private readonly IUnitOfMeasureService _unitOfMeasureService;

    public GetUnitOfMeasureByIdHandler(IUnitOfMeasureService unitOfMeasureService)
    {
        _unitOfMeasureService = unitOfMeasureService;
    }

    public async Task<UnitOfMeasureResult> Handle(GetUnitOfMeasureByIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfMeasureService.GetUnitByIdAsync(request.UomId);
    }
}