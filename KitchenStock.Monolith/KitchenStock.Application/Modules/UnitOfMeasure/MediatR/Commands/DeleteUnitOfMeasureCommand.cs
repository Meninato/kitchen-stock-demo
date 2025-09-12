using KitchenStock.Application.Modules.UnitOfMeasure.Results;
using MediatR;

namespace KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Commands;

public record DeleteUnitOfMeasureCommand(Guid UomId) : IRequest<UnitOfMeasureResult>;