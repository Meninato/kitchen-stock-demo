using KitchenStock.Application.Abstractions;
using KitchenStock.Domain.Entities;

namespace KitchenStock.Application.Modules.UnitOfMeasure.Abstractions;

public interface IUnitOfMeasureRepository : IGenericRepository<UnitOfMeasureEntity>
{
    Task<bool> ExistsBySymbolAsync(string symbol, Guid? excludeId = null);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
    Task<int> GetIngredientCountAsync(Guid unitId);
}
