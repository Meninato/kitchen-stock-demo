namespace Kitchen.Application.Dtos;

public record CreateKitchenDto(
    string Name,
    string? Description,
    bool AllowSharedIngredients = false
);