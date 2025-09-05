using FluentResults;
using User.Application.Dtos;
using User.Application.Results;
using User.Domain.Enums;

namespace User.Application.Services.Abstractions;

public interface IUserService
{
    Task<UserResult> RegisterAsync(RegisterUserDto dto);
    Task<AuthResult> LoginAsync(LoginUserDto dto);
    Task<UserResult> GetByIdAsync(Guid userId);
    Task<UserResult> GetByEmailAsync(string email);
    Task<Result<bool>> CanCreateKitchenAsync(Guid userId, int currentKitchenCount);
    Task<Result<bool>> EmailExistsAsync(string email);
    Task<UserResult> UpdatePlanAsync(Guid userId, UserPlan newPlan);
    Task<Result> DeactivateUserAsync(Guid userId);
}
