using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Application.Modules.Auth.Dtos;
using KitchenStock.Application.Modules.Auth.Results;
using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Application.Security.Vault.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KitchenStock.Infrastructure.Modules.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IVaultService _vaultService;
    private readonly JwtTokenVaultPathSettings _jwtTokenVaultPath;

    public AuthService(
        IUserRepository userRepository, 
        IVaultService vaultService,
        IOptions<KitchenStockSettings> options)
    {
        _userRepository = userRepository;
        _vaultService = vaultService;
        _jwtTokenVaultPath = options.Value.VaultSecretPaths.JwtToken;
    }

    public async Task<AuthResult> AuthenticateAsync(AuthenticateUserDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return AuthResult.Failure(AuthErrors.RequiredFieldsMissing);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return AuthResult.Failure(AuthErrors.InvalidCredentials);

            if (!VerifyPassword(request.Password, user.PasswordHash))
                return AuthResult.Failure(AuthErrors.InvalidCredentials);

            return await IssueTokensAsync(MapToAuthUser(user));
        }
        catch (Exception ex)
        {
            return AuthResult.Failure(AuthErrors.TokenGenerationFailed.CausedBy(ex));
        }
    }

    public async Task<AuthResult> IssueTokensAsync(AuthUserResponseDto user)
    {
        try
        {
            var vaultJwt = await _vaultService.ReadSecretAsync<VaultJwtTokenDto>(_jwtTokenVaultPath.Config);

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

            var expiresAt = DateTime.UtcNow.AddSeconds(vaultJwt.ExpiresInSeconds);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = vaultJwt.Issuer,
                Audience = vaultJwt.Audience,
                Claims = claims,
                Expires = expiresAt,
                SigningCredentials = credentials
            };

            var handler = new JsonWebTokenHandler();
            var tokenString = handler.CreateToken(tokenDescriptor);

            //TODO: create a table RefreshTokens to create a rotation and control

            var authDto = new AuthResponseDto(tokenString, tokenString, expiresAt);
            return AuthResult.Success(authDto);
        }
        catch(Exception ex)
        {
            return AuthResult.Failure(AuthErrors.TokenGenerationFailed.CausedBy(ex));
        }
    }

    public Task<AuthResult> RefreshTokensAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    private AuthUserResponseDto MapToAuthUser(UserEntity user)
    {
        return new AuthUserResponseDto(user.Id, user.Email, user.Name, user.Plan);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
