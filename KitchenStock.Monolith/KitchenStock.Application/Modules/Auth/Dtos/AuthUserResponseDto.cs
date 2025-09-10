using KitchenStock.Domain.Enums;

namespace KitchenStock.Application.Modules.Auth.Dtos;

public record AuthUserResponseDto(Guid Id, string Email, string Name, UserPlan Plan);
