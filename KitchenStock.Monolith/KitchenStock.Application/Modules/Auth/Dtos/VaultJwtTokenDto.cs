namespace KitchenStock.Application.Modules.Auth.Dtos;

public record VaultJwtTokenDto(
    string Secret, 
    string Audience, 
    string Issuer, 
    int ExpiresInSeconds);
