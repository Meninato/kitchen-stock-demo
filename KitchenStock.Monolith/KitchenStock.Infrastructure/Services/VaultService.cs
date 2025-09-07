using KitchenStock.Application.Abstractions;
using System.Text;
using System.Text.Json;

namespace KitchenStock.Infrastructure.Services;

public class VaultService : IVaultService
{
    private readonly HttpClient _httpClient;
    private readonly string _kv2 = "v1/kv/data";
    private readonly JsonSerializerOptions _snakeCaseOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public VaultService(HttpClient httpClient, VaultSettings config)
    {
        if (string.IsNullOrWhiteSpace(config.Url))
            throw new ArgumentException("Vault:Url");

        if (string.IsNullOrWhiteSpace(config.Token))
            throw new ArgumentException("Vault:Token");

        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(config.Url);
        _httpClient.DefaultRequestHeaders.Add("X-Vault-Token", config.Token);
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

public class VaultSettings
{
    public string Url { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
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
