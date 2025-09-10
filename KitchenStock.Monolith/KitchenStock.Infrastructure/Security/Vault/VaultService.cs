using KitchenStock.Application.Abstractions;
using KitchenStock.Infrastructure.Security.Vault.Configuration;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace KitchenStock.Infrastructure.Security.Vault;

public class VaultService : IVaultService
{
    private readonly HttpClient _httpClient;
    private readonly string _kv2 = "v1/kv/data";
    private readonly JsonSerializerOptions _snakeCaseOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public VaultService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> ReadSecretAsync(string path)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_kv2}/{path}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var vaultResponse = JsonSerializer.Deserialize<VaultInternalResponseDto<object>>(json, _snakeCaseOptions);

            if (vaultResponse?.Data?.Data == null)
                return null;

            return JsonSerializer.Serialize(vaultResponse.Data.Data, _snakeCaseOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<T?> ReadSecretAsync<T>(string path)
    {
        var json = await ReadSecretAsync(path);
        if (json == null)
            return default;

        return JsonSerializer.Deserialize<T>(json, _snakeCaseOptions);
    }

    public async Task<string?> ReadSecretValueAsync(string path, string key = "value")
    {
        var json = await ReadSecretAsync(path);
        if (json == null)
            return null;

        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json, _snakeCaseOptions);
        if (dict == null || !dict.TryGetValue(key, out var value))
            return null;

        return value?.ToString();
    }

    public async Task WriteSecretAsync<T>(string path, T data)
    {
        var payload = JsonSerializer.Serialize(new { data }, _snakeCaseOptions);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_kv2}/{path}", content);
        response.EnsureSuccessStatusCode();
    }

    public Task WriteSecretAsync(string path, string data) =>
        WriteSecretAsync(path, new Dictionary<string, string> { ["value"] = data });
}

internal class VaultInternalResponseDto<T>
{
    public VaultDataSectionDto<T>? Data { get; set; }
}

internal class VaultDataSectionDto<T>
{
    public T? Data { get; set; }
    public object? Metadata { get; set; }
}
