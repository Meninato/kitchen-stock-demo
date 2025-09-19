namespace KitchenStock.Application.Modules.Auth.Dtos;

public record AccessTokenDto(
    string TokenValue,
    DateTime UtcExpiresAt
);
