namespace KitchenStock.Application.Abstractions;

public interface IAuthService
{
    Task<string> GenerateTokenAsync(User user);
    Task<User?> AuthenticateAsync(string email, string password);
}