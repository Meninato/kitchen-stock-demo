using KitchenStock.Application.Kitchen.Dtos;
using KitchenStock.Application.Kitchen.Results;

namespace KitchenStock.Application.Abstractions;

public interface IKitchenService
{
    Task<KitchenResult> CreateKitchenAsync(Guid userId, CreateKitchenDto request);
    Task<KitchenResult> GetKitchenByIdAsync(Guid id, Guid userId);
    Task<KitchenListResult> GetUserKitchensAsync(Guid userId);
    Task<KitchenResult> UpdateKitchenAsync(Guid userId, UpdateKitchenDto request);
    Task<KitchenResult> DeleteKitchenAsync(Guid id, Guid userId);
    Task<KitchenStatsResult> GetKitchenStatsAsync(Guid kitchenId, Guid userId);
}
