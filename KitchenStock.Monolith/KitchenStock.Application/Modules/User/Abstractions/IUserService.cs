using KitchenStock.Application.Modules.User.Dtos;
using KitchenStock.Application.Modules.User.Results;

namespace KitchenStock.Application.Modules.User.Abstractions;

public interface IUserService
{
    Task<UserResult> CreateUserAsync(CreateUserDto request);
    Task<UserResult> GetUserByIdAsync(Guid id);
    Task<UserResult> GetUserByEmailAsync(string email);
    Task<UserResult> UpdateUserAsync(Guid id, UpdateUserDto request);
    Task<UserResult> UpdateUserPlanAsync(Guid id, UpdateUserPlanDto request);
    Task<UserResult> DeactivateUserAsync(Guid id);
    Task<bool> CanCreateKitchenAsync(Guid userId);
    Task<bool> EmailExistsAsync(string email);
}
