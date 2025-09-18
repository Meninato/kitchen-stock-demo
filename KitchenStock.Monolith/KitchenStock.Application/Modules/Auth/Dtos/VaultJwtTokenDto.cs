using System.Text.Json.Serialization;

namespace KitchenStock.Application.Modules.Auth.Dtos;

public record VaultJwtTokenDto
{
    public string Secret { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;

    [JsonPropertyName("expires_in_seconds")]
    public int ExpiresInSeconds { get; init; } = 0;
}