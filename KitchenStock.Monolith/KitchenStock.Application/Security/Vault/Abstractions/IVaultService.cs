namespace KitchenStock.Application.Security.Vault.Abstractions;

public interface IVaultService
{
    Task<string?> ReadSecretAsync(string path);
    Task<T?> ReadSecretAsync<T>(string path);
    Task<string?> ReadSecretValueAsync(string path, string key = "value");
    Task WriteSecretAsync<T>(string path, T data);
    Task WriteSecretAsync(string path, string data);
}
