using KitchenStock.Application.Auth.Dtos;
using KitchenStock.Application.Auth.Results;

namespace KitchenStock.Application.Abstractions;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(string email, string password);
}