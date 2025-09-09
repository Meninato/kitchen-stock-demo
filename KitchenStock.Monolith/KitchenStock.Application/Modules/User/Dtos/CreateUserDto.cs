using KitchenStock.Domain.Enums;

namespace KitchenStock.Application.Modules.User.Dtos;

public record CreateUserDto(
    string Name,
    string Email,
    string Password,
    UserPlan Plan = UserPlan.Basic
);
