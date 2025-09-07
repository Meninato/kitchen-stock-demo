using KitchenStock.Domain.Enums;

namespace KitchenStock.Application.User.Dtos;

public record UserResponseDto(
    Guid Id,
    string Name,
    string Email,
    UserPlan Plan,
    DateTime CreatedAt
);
