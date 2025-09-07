namespace KitchenStock.Application.Auth.Dtos;

public record AuthResponseDto(string Token, int ExpiresIn);
