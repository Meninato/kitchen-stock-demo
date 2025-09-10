using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Application.Modules.Auth.Dtos;
using KitchenStock.Application.Modules.Auth.Results;
using KitchenStock.Application.Modules.User.Abstractions;

namespace KitchenStock.Infrastructure.Modules.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResult> AuthenticateAsync(AuthenticateDto request)
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

            var accessToken = GenerateJwtToken(user);

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expirationInSeconds = int.Parse(jwtSettings["ExpirationInMinutes"] ?? "1440");

            var authResponse = new AuthResponseDto(
                accessToken,
                "123456",
                DateTime.UtcNow.AddSeconds(expirationInSeconds)
            );

            return AuthResult.Success(authResponse);
        }
        catch (Exception ex)
        {
            return AuthResult.Failure(AuthErrors.TokenGenerationFailed.CausedBy(ex));
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
