using FluentResults;
using Kitchen.Application.Dtos;
using Kitchen.Application.Results;

namespace Kitchen.Application.Services.Abstractions;

public interface IKitchenService
{
    Task<KitchenResult> CreateKitchenAsync(Guid userId, CreateKitchenDto dto);
    Task<KitchenResult> UpdateKitchenAsync(Guid userId, Guid kitchenId, UpdateKitchenDto dto);
    Task<Result> DeleteKitchenAsync(Guid userId, Guid kitchenId);
    Task<KitchenResult> GetKitchenAsync(Guid userId, Guid kitchenId);
    Task<KitchenListResult> GetUserKitchensAsync(Guid userId, bool includeInactive = false);
    Task<KitchenStatsResult> GetKitchenStatsAsync(Guid userId, Guid kitchenId);
    Task<Result> ActivateKitchenAsync(Guid userId, Guid kitchenId);
    Task<Result> DeactivateKitchenAsync(Guid userId, Guid kitchenId);
}
