using FluentResults;

namespace Kitchen.Application.Services.Abstractions;

public interface IUserApiClient
{
    Task<Result<UserInfo>> GetUserAsync(Guid userId);
    Task<Result<bool>> CanCreateKitchenAsync(Guid userId, int currentKitchenCount);
    Task<Result<bool>> ValidateUserPlanAsync(Guid userId, string requiredFeature);
}

public record UserInfo(
    Guid Id,
    string Name,
    string Email,
    string Plan,
    int MaxKitchens
);