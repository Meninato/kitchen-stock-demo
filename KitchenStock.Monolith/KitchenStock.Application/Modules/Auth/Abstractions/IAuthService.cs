using FluentResults;
using KitchenStock.Application.Modules.Auth.Dtos;
using KitchenStock.Application.Modules.Auth.Results;

namespace KitchenStock.Application.Modules.Auth.Abstractions;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(AuthenticateUserDto request, string ipAddress, string userAgent);
    Task<AuthResult> RefreshTokenAsync(string refreshToken, string ipAddress, string userAgent);
    Task<Result> LogoutAsync(string refreshToken, string reason = "User logout");
    Task<Result> RevokeAllUserTokensAsync(Guid userId, string reason = "Security revocation");

    Task<AccessTokenDto> GenerateJwtTokenAsync(AuthUserResponseDto user);
    Task<RefreshTokenDto> GenerateRefreshTokenAsync(Guid userId, string ipAddress, string userAgent);
}
