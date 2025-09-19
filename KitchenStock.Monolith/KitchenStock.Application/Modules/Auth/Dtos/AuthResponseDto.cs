namespace KitchenStock.Application.Modules.Auth.Dtos;

public record AuthResponseDto(
    AccessTokenDto AccessToken,
    RefreshTokenDto RefreshToken
);
