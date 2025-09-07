using FluentResults;
using KitchenStock.Application.User.Dtos;
using KitchenStock.Application.User.Results;

namespace KitchenStock.Application.Abstractions;

public interface IUserService
{
    Task<KitchenResult> RegisterAsync(RegisterUserDto dto);
    Task<KitchenResult> UpdateAsync(Guid userId, UpdateUserDto dto);
    Task<KitchenResult> GetByIdAsync(Guid userId);
    Task<KitchenResult> GetByEmailAsync(string email);
    Task<Result<bool>> EmailExistsAsync(string email);
    Task<Result> RemoveAsync(Guid userId);
}
