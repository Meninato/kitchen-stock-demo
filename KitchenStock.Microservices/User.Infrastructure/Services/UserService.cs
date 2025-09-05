using FluentResults;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using User.Application.Dtos;
using User.Application.Errors;
using User.Application.Results;
using User.Application.Services.Abstractions;
using User.Domain.Entities;
using User.Domain.Enums;

namespace User.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResult> RegisterAsync(RegisterUserDto dto)
    {
        try
        {
            var validationResult = ValidateUserRegistration(dto);
            if (validationResult.IsFailed)
                return UserResult.Failure(validationResult.Errors);

            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

            if (await _userRepository.ExistsAsync(normalizedEmail))
            {
                return UserResult.Failure(UserErrors.Registration.EmailAlreadyExists(normalizedEmail));
            }

            if (!IsPasswordStrong(dto.Password))
            {
                return UserResult.Failure(UserErrors.Registration.PasswordTooWeak);
            }

            var user = new UserEntity
            {
                Name = dto.Name.Trim(),
                Email = normalizedEmail,
                PasswordHash = HashPassword(dto.Password),
                Plan = dto.UserPlan
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var userDto = MapToDto(createdUser);

            return UserResult.Success(userDto);
        }
        catch(Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("user registration", ex));
        }
    }

    public async Task<AuthResult> LoginAsync(LoginUserDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return AuthResult.Failure(UserErrors.Authentication.InvalidCredentials);
            }

            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.GetByEmailAsync(normalizedEmail);
            if (user == null)
            {
                return AuthResult.Failure(UserErrors.Authentication.InvalidCredentials);
            }

            if (user.IsDeleted)
            {
                return AuthResult.Failure(UserErrors.Authentication.UserDeactivated);
            }

            if (!VerifyPassword(dto.Password, user.PasswordHash))
            {
                return AuthResult.Failure(UserErrors.Authentication.InvalidCredentials);
            }

            var tokenResult = GenerateJwtToken(user);
            if (tokenResult.IsFailed)
            {
                return AuthResult.Failure(tokenResult.Errors);
            }

            return AuthResult.Success(tokenResult.Value);
        }
        catch(Exception ex)
        {
            return AuthResult.Failure(UserErrors.UnexpectedError("user login", ex));
        }
    }

    public async Task<UserResult> GetByIdAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return UserResult.Failure(UserErrors.UserNotFound(userId));
            }

            var userDto = MapToDto(user);
            return UserResult.Success(userDto);
        }
        catch (Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("get user by id", ex));
        }
    }

    public async Task<UserResult> GetByEmailAsync(string email)
    {
        try
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (user == null)
            {
                return UserResult.Failure(UserErrors.Authentication.UserNotFound(normalizedEmail));
            }

            var userDto = MapToDto(user);
            return UserResult.Success(userDto);
        }
        catch (Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("get user by email", ex));
        }
    }

    public async Task<Result<bool>> CanCreateKitchenAsync(Guid userId, int currentKitchenCount)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result.Fail(UserErrors.UserNotFound(userId));
            }

            if (currentKitchenCount >= user.MaxKitchens)
            {
                return Result.Fail(UserErrors.BusinessRules.CannotCreateMoreKitchens(user.MaxKitchens, currentKitchenCount));
            }

            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            return Result.Fail(UserErrors.UnexpectedError("check kitchen creation", ex));
        }
    }

    public async Task<Result<bool>> EmailExistsAsync(string email)
    {
        try
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var exists = await _userRepository.ExistsAsync(normalizedEmail);
            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail(UserErrors.UnexpectedError("check email existence", ex));
        }
    }

    public async Task<UserResult> UpdatePlanAsync(Guid userId, UserPlan newPlan)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return UserResult.Failure(UserErrors.UserNotFound(userId));
            }

            if (!Enum.IsDefined(typeof(UserPlan), newPlan))
            {
                return UserResult.Failure(UserErrors.BusinessRules.PlanNotAvailable(newPlan.ToString()));
            }

            var maxKitchensNewPlan = GetMaxKitchensForPlan(newPlan);
            if (maxKitchensNewPlan < user.MaxKitchens)
            {
                //TODO: Call Kitchen.API to check how many kitchens exists
                return UserResult.Failure(UserErrors.BusinessRules.CannotDowngradePlan(
                    user.Plan.ToString(),
                    newPlan.ToString(),
                    "User has more kitchens than allowed in new plan"
                ));
            }

            user.Plan = newPlan;
            var updatedUser = await _userRepository.UpdateAsync(user);
            var userDto = MapToDto(updatedUser);

            return UserResult.Success(userDto);
        }
        catch (Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("update user plan", ex));
        }
    }

    public async Task<Result> DeactivateUserAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return Result.Fail(UserErrors.UserNotFound(userId));
            }

            //TODO: Could transfer kitchens, cancel subscription, clean sensitive data

            var success = await _userRepository.DeleteAsync(userId); // Soft delete

            if (!success)
            {
                return Result.Fail(UserErrors.DatabaseError("user deactivation"));
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(UserErrors.UnexpectedError("user deactivation", ex));
        }
    }

    private int GetMaxKitchensForPlan(UserPlan plan) => plan switch
    {
        UserPlan.Basic => 1,
        UserPlan.Premium => 5,
        _ => 1
    };

    private Result ValidateUserRegistration(RegisterUserDto dto)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add(UserErrors.Registration.NameRequired);
        else if (dto.Name.Length > 100)
            errors.Add(UserErrors.Registration.NameTooLong(100));

        if (string.IsNullOrWhiteSpace(dto.Email))
            errors.Add(UserErrors.Registration.InvalidEmail(dto.Email ?? ""));
        else if (!IsValidEmail(dto.Email))
            errors.Add(UserErrors.Registration.InvalidEmail(dto.Email));

        if (string.IsNullOrWhiteSpace(dto.Password))
            errors.Add(UserErrors.Registration.PasswordTooWeak);

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private bool IsValidEmail(string email)
    {
        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailRegex);
    }

    private bool IsPasswordStrong(string password)
    {
        if (password.Length < 6) return false;

        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasNumber = password.Any(char.IsDigit);

        return hasUpper && hasLower && hasNumber;
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    private UserResponseDto MapToDto(UserEntity user)
    {
        return new UserResponseDto(
            user.Id,
            user.Name,
            user.Email,
            user.Plan,
            user.MaxKitchens,
            user.CreatedAt
        );
    }

    private Result<string> GenerateJwtToken(UserEntity user)
    {
        try
        {
            //TODO: use a vault service

            var secret = "123456";//_configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(secret))
            {
                return Result.Fail(UserErrors.Authentication.TokenGenerationFailed);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new Dictionary<string, object>()
            {
                [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
                ["email"] = user.Email,
                ["name"] = user.Name,
                ["plan"] = user.Plan.ToString(),
                ["maxKitchens"] = user.MaxKitchens.ToString(),
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
                [JwtRegisteredClaimNames.Iat] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "myapp.com",
                Audience = "myapp-clients",
                Claims = claims,
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = credentials
            };

            var handler = new JsonWebTokenHandler();
            var tokenString = handler.CreateToken(tokenDescriptor);

            return Result.Ok(tokenString);
        }
        catch (Exception ex)
        {
            return Result.Fail(UserErrors.Authentication.TokenGenerationFailed.CausedBy(ex));
        }
    }
}
