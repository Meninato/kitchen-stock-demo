using FluentResults;
using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Application.Modules.User.Dtos;
using KitchenStock.Application.Modules.User.Results;
using KitchenStock.Domain.Entities;
using KitchenStock.Domain.Enums;
using System.Text.RegularExpressions;

namespace KitchenStock.Infrastructure.Modules.User.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResult> CreateUserAsync(CreateUserDto request)
    {
        try
        {
            var validationResult = ValidateCreateUserRequest(request);
            if (validationResult.IsFailed)
                return UserResult.Failure(validationResult.Errors);

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            if (await _userRepository.ExistsAsync(normalizedEmail))
                return UserResult.Failure(UserErrors.Registration.EmailAlreadyExists(normalizedEmail));

            if (!IsPasswordStrong(request.Password))
                return UserResult.Failure(UserErrors.Registration.PasswordTooWeak);

            var user = new UserEntity
            {
                Name = request.Name.Trim(),
                Email = normalizedEmail,
                PasswordHash = HashPassword(request.Password),
                Plan = request.Plan,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var userDto = MapToResponse(createdUser, 0);

            return UserResult.Success(userDto);
        }
        catch (Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("user registration", ex));
        }
    }

    public async Task<UserResult> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return UserResult.Failure(UserErrors.UserNotFound(id));

            var kitchenCount = await _userRepository.GetKitchenCountAsync(id);
            var response = MapToResponse(user, kitchenCount);

            return UserResult.Success(response);
        }
        catch (Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("get user by id", ex));
        }
    }

    public async Task<UserResult> GetUserByEmailAsync(string email)
    {
        try
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (user == null)
                return UserResult.Failure(UserErrors.UserNotFound(email));

            var kitchenCount = await _userRepository.GetKitchenCountAsync(user.Id);
            var response = MapToResponse(user, kitchenCount);

            return UserResult.Success(response);
        }
        catch (Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("get user by email", ex));
        }
    }

    public async Task<UserResult> UpdateUserAsync(Guid id, UpdateUserDto request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return UserResult.Failure(UserErrors.UserNotFound(id));

            var validationResult = ValidateUpdateUserRequest(request);
            if (validationResult.IsFailed)
                return UserResult.Failure(validationResult.Errors);

            // Verificar se email não está sendo usado por outro usuário
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null && existingUser.Id != id)
                return UserResult.Failure(UserErrors.Registration.EmailAlreadyExists(request.Email));

            user.Name = request.Name.Trim();
            user.Email = request.Email.Trim().ToLowerInvariant();

            var updatedUser = await _userRepository.UpdateAsync(user);
            var kitchenCount = await _userRepository.GetKitchenCountAsync(id);
            var response = MapToResponse(updatedUser, kitchenCount);

            return UserResult.Success(response);
        }
        catch (Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("user update", ex));
        }
    }

    public async Task<UserResult> UpdateUserPlanAsync(Guid id, UpdateUserPlanDto request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return UserResult.Failure(UserErrors.UserNotFound(id));

            var currentKitchens = await _userRepository.GetKitchenCountAsync(id);
            var newMaxKitchens = GetMaxKitchensForPlan(request.Plan);

            if (currentKitchens > newMaxKitchens)
                return UserResult.Failure(UserErrors.BusinessRules.CannotDowngradePlan(
                    user.Plan.ToString(), request.Plan.ToString(), "User has more kitchens than allowed in new plan"));

            user.Plan = request.Plan;
            var updatedUser = await _userRepository.UpdateAsync(user);
            var response = MapToResponse(updatedUser, currentKitchens);

            return UserResult.Success(response);
        }
        catch (Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("update user plan", ex));
        }
    }

    public async Task<UserResult> DeactivateUserAsync(Guid id)
    {
        try
        {
            var success = await _userRepository.DeleteAsync(id);
            if (!success)
                return UserResult.Failure(UserErrors.UserNotFound(id));

            return UserResult.Success();
        }
        catch (Exception ex)
        {
            return UserResult.Failure(UserErrors.UnexpectedError("user deactivation", ex));
        }
    }

    public async Task<bool> CanCreateKitchenAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        var currentKitchens = await _userRepository.GetKitchenCountAsync(userId);
        return currentKitchens < 5;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userRepository.ExistsAsync(email);
    }

    private Result ValidateCreateUserRequest(CreateUserDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(UserErrors.Registration.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(UserErrors.Registration.NameTooLong(100));

        if (string.IsNullOrWhiteSpace(request.Email))
            errors.Add(UserErrors.Registration.InvalidEmail(request.Email ?? ""));
        else if (!IsValidEmail(request.Email))
            errors.Add(UserErrors.Registration.InvalidEmail(request.Email));

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private Result ValidateUpdateUserRequest(UpdateUserDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(UserErrors.Registration.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(UserErrors.Registration.NameTooLong(100));

        if (string.IsNullOrWhiteSpace(request.Email))
            errors.Add(UserErrors.Registration.InvalidEmail(request.Email ?? ""));
        else if (!IsValidEmail(request.Email))
            errors.Add(UserErrors.Registration.InvalidEmail(request.Email));

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

    private int GetMaxKitchensForPlan(UserPlan plan) => plan switch
    {
        UserPlan.Basic => 1,
        UserPlan.Premium => 5,
        _ => 1
    };

    private UserResponseDto MapToResponse(UserEntity user, int currentKitchens)
    {
        return new UserResponseDto(
            user.Id,
            user.Name,
            user.Email,
            user.Plan,
            currentKitchens,
            user.CreatedAt
        );
    }
}
