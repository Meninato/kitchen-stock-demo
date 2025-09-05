using User.Application.Dtos;

namespace User.Application.Services.Abstractions;

public interface IUserService
{
    Task<UserResponseDto?> RegisterAsync(RegisterUserDto dto);
    Task<string?> LoginAsync(LoginUserDto dto);
    Task<UserResponseDto?> GetByIdAsync(Guid id);
    Task<UserResponseDto?> GetByEmailAsync(string email);
    Task<bool> CanCreateKitchenAsync(Guid userId, int currentKitchenCount);
}
