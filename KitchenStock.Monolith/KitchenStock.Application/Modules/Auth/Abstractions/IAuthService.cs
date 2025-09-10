using KitchenStock.Application.Modules.Auth.Dtos;
using KitchenStock.Application.Modules.Auth.Results;

namespace KitchenStock.Application.Modules.Auth.Abstractions;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(AuthenticateUserDto request);
    Task<AuthResult> IssueTokensAsync(AuthUserResponseDto user);
    Task<AuthResult> RefreshTokensAsync(string refreshToken);
}
