using KitchenStock.Application.Modules.UnitOfMeasure.Dtos;
using KitchenStock.Application.Modules.UnitOfMeasure.Results;

namespace KitchenStock.Application.Modules.UnitOfMeasure.Abstractions;

public interface IUnitOfMeasureService
{
    Task<UnitOfMeasureListResult> GetAllUnitsAsync();
    Task<UnitOfMeasureResult> GetUnitByIdAsync(Guid id);
    Task<UnitOfMeasureResult> CreateUnitAsync(CreateUnitOfMeasureDto request);
    Task<UnitOfMeasureResult> UpdateUnitAsync(Guid id, UpdateUnitOfMeasureDto request);
    Task<UnitOfMeasureResult> DeleteUnitAsync(Guid id);
}