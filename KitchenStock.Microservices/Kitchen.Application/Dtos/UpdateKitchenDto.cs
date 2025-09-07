namespace Kitchen.Application.Dtos;

public record UpdateKitchenDto(
    string Name,
    string? Description,
    bool AllowSharedIngredients = false
);