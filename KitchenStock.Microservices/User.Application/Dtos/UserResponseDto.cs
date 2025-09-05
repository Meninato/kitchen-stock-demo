using User.Domain.Enums;

namespace User.Application.Dtos;
public record UserResponseDto(
    Guid Id,
    string Name,
    string Email,
    UserPlan Plan,
    int MaxKitchens,
    DateTime CreatedAt
);
