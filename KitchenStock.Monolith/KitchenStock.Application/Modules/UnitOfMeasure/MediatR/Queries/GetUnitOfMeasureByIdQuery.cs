using KitchenStock.Application.Modules.UnitOfMeasure.Results;
using MediatR;

namespace KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Queries;

public record GetUnitOfMeasureByIdQuery(Guid UomId) : IRequest<UnitOfMeasureResult>;