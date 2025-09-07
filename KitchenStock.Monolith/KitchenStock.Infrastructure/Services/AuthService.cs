using FluentResults;
using KitchenStock.Application.Abstractions;
using KitchenStock.Application.Auth.Dtos;
using KitchenStock.Application.Auth.Results;
using KitchenStock.Application.Errors;
using KitchenStock.Domain.Entities;
using KitchenStock.Infrastructure.Services.Vault.Dtos;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KitchenStock.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IVaultService _vaultService;

    public AuthService(IUserRepository userRepository, IVaultService vaultService)
    {
        _userRepository = userRepository;
        _vaultService = vaultService;
    }

    public async Task<AuthResult> AuthenticateAsync(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return AuthResult.Failure(AuthErrors.Authentication.InvalidCredentials);

            var normalizedEmail = email.Trim().ToLowerInvariant();

            var user = await _userRepository.GetByEmailAsync(normalizedEmail);
            if (user == null)
                return AuthResult.Failure(AuthErrors.Authentication.InvalidCredentials);

            if (!VerifyPassword(password, user.PasswordHash))
                return AuthResult.Failure(AuthErrors.Authentication.InvalidCredentials);

            var tokenResult = await GenerateJwtTokenAsync(user);
            if (tokenResult.IsFailed)
            {
                return AuthResult.Failure(tokenResult.Errors);
            }

            return AuthResult.Success(tokenResult.Value);
        }
        catch (Exception ex)
        {
            return AuthResult.Failure(AuthErrors.UnexpectedError("user login", ex));
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    private async Task<AuthResult> GenerateJwtTokenAsync(UserEntity user)
    {
        try
        {
            //TODO: use a vault service and cache secret for a couple of minutes to avoid lot of access
            //TODO: don't hardcode vault path for auth service

            var vaultJwt = await _vaultService.ReadSecretAsync<VaultJwtDto>("kitchen/services/jwt");
            if (vaultJwt != null && string.IsNullOrEmpty(vaultJwt.Secret))
                return AuthResult.Failure(AuthErrors.Authentication.TokenGenerationFailed);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(vaultJwt!.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new Dictionary<string, object>()
            {
                [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
                ["email"] = user.Email,
                ["name"] = user.Name,
                ["plan"] = user.Plan.ToString(),
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
                [JwtRegisteredClaimNames.Iat] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            int expiresInSeconds = 60 * 60 * 6;
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "myapp.com",
                Audience = "myapp-clients",
                Claims = claims,
                Expires = DateTime.UtcNow.AddSeconds(expiresInSeconds),
                SigningCredentials = credentials
            };

            var handler = new JsonWebTokenHandler();
            var tokenString = handler.CreateToken(tokenDescriptor);
            var authDto = new AuthResponseDto(tokenString, expiresInSeconds);

            return AuthResult.Success(authDto);
        }
        catch (Exception ex)
        {
            return AuthResult.Failure(AuthErrors.Authentication.TokenGenerationFailed.CausedBy(ex));
        }
    }
}
