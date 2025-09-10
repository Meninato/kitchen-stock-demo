namespace KitchenStock.Application.Modules.Auth.Dtos;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
