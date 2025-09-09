using KitchenStock.Domain.Enums;

namespace KitchenStock.Application.Modules.User.Dtos;

public record UserResponseDto(
    Guid Id,
    string Name,
    string Email,
    UserPlan Plan,
    int CurrentKitchens,
    DateTime CreatedAt
);
