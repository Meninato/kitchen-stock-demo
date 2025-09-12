namespace KitchenStock.Application.Modules.Recipe.Dtos;

public record CreateRecipeDto(
    Guid KitchenId,
    string Name,
    string Description,
    decimal Yield,
    int PrepTimeMinutes,
    string Instructions,
    decimal? SellingPrice,
    List<RecipeIngredientDto> Ingredients
);
