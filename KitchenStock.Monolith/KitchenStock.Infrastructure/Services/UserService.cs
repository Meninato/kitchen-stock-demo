using FluentResults;
using KitchenStock.Application.Abstractions;
using KitchenStock.Application.Errors;
using KitchenStock.Application.User.Dtos;
using KitchenStock.Application.User.Results;
using KitchenStock.Domain.Entities;
using System.Text.RegularExpressions;

namespace KitchenStock.Infrastructure.Services;

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
        catch (Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("user registration", ex));
        }
    }

    public async Task<UserResult> UpdateAsync(Guid userId, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return UserResult.Failure(UserErrors.UserNotFound(userId));
        }

        var validationResult = ValidateUserUpdate(dto);
        if (validationResult.IsFailed)
            return UserResult.Failure(validationResult.Errors);

        user.Name = dto.Name.Trim();

        var updatedUser = await _userRepository.UpdateAsync(user);
        var userDto = MapToDto(updatedUser);

        return UserResult.Success(userDto);
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
                return UserResult.Failure(UserErrors.UserNotFound(normalizedEmail));
            }

            var userDto = MapToDto(user);
            return UserResult.Success(userDto);
        }
        catch (Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("get user by email", ex));
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

    public async Task<Result> RemoveAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result.Fail(UserErrors.UserNotFound(userId));
            }

            //TODO: Could transfer kitchens, cancel subscription, clean sensitive data

            var success = await _userRepository.DeleteAsync(userId);
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

    private Result ValidateUserUpdate(UpdateUserDto dto)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add(UserErrors.Registration.NameRequired);
        else if (dto.Name.Length > 100)
            errors.Add(UserErrors.Registration.NameTooLong(100));

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

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

    private UserResponseDto MapToDto(UserEntity user)
    {
        return new UserResponseDto(
            user.Id,
            user.Name,
            user.Email,
            user.Plan,
            user.CreatedAt
        );
    }
}
