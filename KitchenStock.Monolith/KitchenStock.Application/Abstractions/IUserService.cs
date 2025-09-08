using FluentResults;
using KitchenStock.Application.User.Dtos;
using KitchenStock.Application.User.Results;

namespace KitchenStock.Application.Abstractions;

public interface IUserService
{
    Task<UserResult> RegisterAsync(RegisterUserDto dto);
    Task<UserResult> UpdateAsync(Guid userId, UpdateUserDto dto);
    Task<UserResult> GetByIdAsync(Guid userId);
    Task<UserResult> GetByEmailAsync(string email);
    Task<Result<bool>> EmailExistsAsync(string email);
    Task<Result> RemoveAsync(Guid userId);
}
