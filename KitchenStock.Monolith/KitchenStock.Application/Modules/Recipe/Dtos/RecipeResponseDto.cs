namespace KitchenStock.Application.Modules.Recipe.Dtos;

public record RecipeResponseDto(
    Guid Id,
    string Name,
    string Description,
    decimal Yield,
    int PrepTimeMinutes,
    string Instructions,
    decimal? SellingPrice,
    decimal TotalCost,
    decimal CostPerPortion,
    decimal? ProfitMargin,
    List<RecipeIngredientResponseDto> Ingredients,
    DateTime CreatedAt
);