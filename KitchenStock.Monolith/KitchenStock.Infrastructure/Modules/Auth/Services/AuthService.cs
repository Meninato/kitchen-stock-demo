using FluentResults;
using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Application.Modules.Auth.Dtos;
using KitchenStock.Application.Modules.Auth.Results;
using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Application.Modules.User.Dtos;
using KitchenStock.Application.Modules.User.MediatR.Commands;
using KitchenStock.Application.Security.Vault.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace KitchenStock.Infrastructure.Modules.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IVaultService _vaultService;
    private readonly JwtTokenVaultPathSettings _jwtTokenVaultPath;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IVaultService vaultService,
        IOptions<KitchenStockSettings> options)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _vaultService = vaultService;
        _jwtTokenVaultPath = options.Value.VaultSecretPaths.JwtToken;
    }

    public async Task<Result> LogoutAsync(string refreshTokenValue, string reason = "User logout")
    {
        try
        {
            var tokenHash = HashToken(refreshTokenValue);
            await _refreshTokenRepository.RevokeTokenAsync(tokenHash, reason);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Logout failed: {ex.Message}");
        }
    }

    public async Task<Result> RevokeAllUserTokensAsync(Guid userId, string reason = "Security revocation")
    {
        try
        {
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId, reason);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Token revocation failed: {ex.Message}");
        }
    }

    public async Task<AuthResult> AuthenticateAsync(AuthenticateUserDto request, string ipAddress, string userAgent)
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

            var accessToken = await GenerateJwtTokenAsync(MapToAuthUser(user));
            var refreshToken = await GenerateRefreshTokenAsync(user.Id, ipAddress, userAgent);

            var authResponse = new AuthResponseDto(accessToken, refreshToken);

            return AuthResult.Success(authResponse);
        }
        catch (Exception ex)
        {
            return AuthResult.Failure(AuthErrors.TokenGenerationFailed.CausedBy(ex));
        }
    }

    public async Task<AccessTokenDto> GenerateJwtTokenAsync(AuthUserResponseDto user)
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

        return new AccessTokenDto(tokenString, expiresAt);
    }

    public async Task<RefreshTokenDto> GenerateRefreshTokenAsync(Guid userId, string ipAddress, string userAgent)
    {
        // Generate cryptographically secure random token
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var tokenValue = Convert.ToBase64String(randomBytes);

        var refreshToken = new RefreshTokenEntity
        {
            UserId = userId,
            TokenHash = HashToken(tokenValue),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        await _refreshTokenRepository.CreateAsync(refreshToken);

        return new RefreshTokenDto(tokenValue, refreshToken.ExpiresAt);
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshTokenValue, string ipAddress, string userAgent)
    {
        try
        {
            var tokenHash = HashToken(refreshTokenValue);
            var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

            if (refreshToken == null)
                return AuthResult.Failure(AuthErrors.RefreshTokenNotFound);

            if (refreshToken.IsRevoked)
            {
                // Token rotation - revoke all user tokens if a revoked token is used
                await _refreshTokenRepository.RevokeAllUserTokensAsync(
                    refreshToken.UserId,
                    "Attempted reuse of revoked token");

                return AuthResult.Failure(AuthErrors.RefreshTokenRevoked);
            }

            if (refreshToken.IsExpired)
                return AuthResult.Failure(AuthErrors.RefreshTokenExpired);

            var user = refreshToken.User;
            if (user == null)
                return AuthResult.Failure(AuthErrors.UserNotFound(""));

            // Generate new tokens
            var newAccessToken = await GenerateJwtTokenAsync(MapToAuthUser(user));
            var newRefreshToken = await GenerateRefreshTokenAsync(user.Id, ipAddress, userAgent);

            // Revoke old refresh token (token rotation)
            await _refreshTokenRepository.RevokeTokenAsync(
                tokenHash,
                "Replaced by new token",
                HashToken(newRefreshToken.TokenValue));

            var authResponse = new AuthResponseDto(newAccessToken, newRefreshToken);

            return AuthResult.Success(authResponse);
        }
        catch (Exception ex)
        {
            return AuthResult.Failure(AuthErrors.TokenGenerationFailed.CausedBy(ex));
        }
    }

    private AuthUserResponseDto MapToAuthUser(UserEntity user)
    {
        return new AuthUserResponseDto(user.Id, user.Email, user.Name, user.Plan);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    private string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
    }
}
