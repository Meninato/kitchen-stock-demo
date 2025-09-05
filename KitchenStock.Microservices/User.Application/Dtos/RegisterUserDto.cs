using User.Domain.Enums;

namespace User.Application.Dtos;

public record RegisterUserDto(string Name, string Email, string Password, UserPlan UserPlan = UserPlan.Basic);
