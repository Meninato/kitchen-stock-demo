using KitchenStock.Application.Modules.UnitOfMeasure.Results;
using MediatR;

namespace KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Queries;

public record GetAllUnitOfMeasureQuery() : IRequest<UnitOfMeasureListResult>;