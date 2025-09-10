using KitchenStock.Application.Modules.Auth.Dtos;
using KitchenStock.Application.Modules.Auth.Results;

namespace KitchenStock.Application.Modules.Auth.Abstractions;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(AuthenticateDto request);
    Task<string> GenerateJwtToken(AuthUserResponseDto user);
}
