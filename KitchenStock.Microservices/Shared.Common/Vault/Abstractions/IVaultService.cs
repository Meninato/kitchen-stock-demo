using Shared.Common.Vault.Dtos;

namespace Shared.Common.Vault.Abstractions;

public interface IVaultService
{
    Task<TResponse?> GetAsync<TResponse>(string path);
    Task<VaultResponseDto<TData, TMeta>?> GetWithMetadataAsync<TData, TMeta>(string path);
    Task<bool> SetAsync<T>(string path, T data);
}
