using KitchenStock.Application.Kitchen.Dtos;
using KitchenStock.Application.Kitchen.Results;

namespace KitchenStock.Application.Abstractions;

public interface IKitchenService
{
    Task<KitchenResult> CreateKitchenAsync(CreateKitchenDto request);
    Task<KitchenResult> GetKitchenByIdAsync(Guid id, Guid userId);
    Task<KitchenListResult> GetUserKitchensAsync(Guid userId);
    Task<KitchenResult> UpdateKitchenAsync(UpdateKitchenDto request);
    Task<KitchenResult> DeleteKitchenAsync(Guid id, Guid userId);
    Task<KitchenStatsResult> GetKitchenStatsAsync(Guid kitchenId, Guid userId);
}
