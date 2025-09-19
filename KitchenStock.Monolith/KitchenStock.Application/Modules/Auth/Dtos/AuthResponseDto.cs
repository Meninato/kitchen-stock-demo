namespace KitchenStock.Application.Modules.Auth.Dtos;

public record AuthResponseDto(
    string AccessToken,
    DateTime AccessTokenUtcExpiresAt,
    string RefreshToken
);
