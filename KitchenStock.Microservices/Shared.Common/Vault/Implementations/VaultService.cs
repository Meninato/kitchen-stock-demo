using Flurl.Http;
using Flurl.Http.Configuration;
using Shared.Common.Vault.Abstractions;
using Shared.Common.Vault.Dtos;
using Shared.Common.Vault.Settings;
using System.Text.Json;

namespace Shared.Common.Vault.Implementations;

public class VaultService : IVaultService
{
    private readonly IFlurlClientCache _flurlClientCache;
    private readonly string _vaultUrl;
    private readonly string _vaultToken;
    private readonly string _kv2 = "v1/kv/data";
    private readonly JsonSerializerOptions jsonSerializerOptionsSnakeCase = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public VaultService(IFlurlClientCache flurlClientCache, VaultSettings config)
    {
        _flurlClientCache = flurlClientCache;
        _vaultUrl = string.IsNullOrWhiteSpace(config.Url)
            ? throw new ArgumentException("Vault:Url")
            : config.Url;

        _vaultToken = string.IsNullOrWhiteSpace(config.Token)
            ? throw new ArgumentException("Vault:Token")
            : config.Token;
    }

    public async Task<TResponse?> GetAsync<TResponse>(string path)
    {
        try
        {
            var client = GetHttpClient();

            var vaultResponse = await client
                .Request(_kv2, path)
                .GetJsonAsync<VaultInternalResponseDto<object>>();

            if (vaultResponse?.Data?.Data == null)
                return default(TResponse);

            // Convert the nested data to the requested type
            var dataJson = JsonSerializer.Serialize(vaultResponse.Data.Data);
            return JsonSerializer.Deserialize<TResponse>(dataJson, jsonSerializerOptionsSnakeCase);
        }
        catch (FlurlHttpException)
        {
            return default(TResponse);
        }
    }

    public async Task<VaultResponseDto<TData, TMeta>?> GetWithMetadataAsync<TData, TMeta>(string path)
    {
        try
        {
            var client = GetHttpClient();
            var vaultResponse = await client
                .Request(_kv2, path)
                .GetJsonAsync<VaultInternalResponseDto<object>>();

            if (vaultResponse?.Data == null)
                return null;

            // Convert data
            var dataJson = JsonSerializer.Serialize(vaultResponse.Data.Data);
            var data = JsonSerializer.Deserialize<TData>(dataJson, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            // Convert metadata
            var metadataJson = JsonSerializer.Serialize(vaultResponse.Data.Metadata);
            var metadata = JsonSerializer.Deserialize<TMeta>(metadataJson, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            return new VaultResponseDto<TData, TMeta>
            {
                Data = data,
                Metadata = metadata
            };
        }
        catch (FlurlHttpException)
        {
            return null;
        }
    }

    public async Task<bool> SetAsync<T>(string path, T data)
    {
        try
        {
            var payload = new { data = data };

            var client = GetHttpClient();

            await client
                .Request(_kv2, path)
                .PostJsonAsync(payload);

            return true;
        }
        catch (FlurlHttpException)
        {
            return false;
        }
    }

    private IFlurlClient GetHttpClient()
    {
        return _flurlClientCache.GetOrAdd("vault", _vaultUrl, (builder) =>
        {
            builder.WithHeader("X-Vault-Token", _vaultToken);
        });
    }
}