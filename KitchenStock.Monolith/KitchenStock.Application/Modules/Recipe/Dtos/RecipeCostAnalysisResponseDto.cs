namespace KitchenStock.Application.Modules.Recipe.Dtos;

public record RecipeCostAnalysisResponseDto(
    Guid RecipeId,
    string RecipeName,
    decimal TotalCost,
    decimal CostPerPortion,
    decimal? SellingPrice,
    decimal? ProfitPerPortion,
    decimal? ProfitMarginPercent,
    List<IngredientCostBreakdownDto> CostBreakdown
);