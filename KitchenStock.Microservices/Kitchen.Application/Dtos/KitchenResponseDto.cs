namespace Kitchen.Application.Dtos;

public record KitchenResponseDto(
    Guid Id,
    string Name,
    string? Description,
    Guid OwnerId,
    bool AllowSharedIngredients,
    bool IsActive,
    int TotalIngredients,
    int TotalRecipes,
    int TotalSuppliers,
    DateTime? LastActivityAt
);
