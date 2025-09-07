namespace KitchenStock.Application.Abstractions;

public interface IVaultService
{
    Task<string?> ReadSecretAsync(string path);
    Task<T?> ReadSecretAsync<T>(string path);
    Task WriteSecretAsync<T>(string path, T data);
    Task WriteSecretAsync(string path, string data);
}