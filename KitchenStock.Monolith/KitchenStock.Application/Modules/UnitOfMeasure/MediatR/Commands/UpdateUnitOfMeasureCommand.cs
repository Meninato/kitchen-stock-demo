using KitchenStock.Application.Modules.UnitOfMeasure.Dtos;
using KitchenStock.Application.Modules.UnitOfMeasure.Results;
using MediatR;

namespace KitchenStock.Application.Modules.UnitOfMeasure.MediatR.Commands;

public record UpdateUnitOfMeasureCommand(Guid UomId, UpdateUnitOfMeasureDto Dto) : IRequest<UnitOfMeasureResult>;