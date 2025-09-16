using FluentResults;
using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Ingredients.Results;
using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Recipe.Abstractions;
using KitchenStock.Application.Modules.Recipe.Dtos;
using KitchenStock.Application.Modules.Recipe.Results;
using KitchenStock.Application.Modules.Stock.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Domain.Enums;

namespace KitchenStock.Infrastructure.Modules.Recipe.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IKitchenRepository _kitchenRepository;
    private readonly IStockEntryRepository _stockRepository;

    public RecipeService(
        IRecipeRepository recipeRepository,
        IIngredientRepository ingredientRepository,
        IKitchenRepository kitchenRepository,
        IStockEntryRepository stockRepository)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _kitchenRepository = kitchenRepository;
        _stockRepository = stockRepository;
    }

    public async Task<RecipeResult> CreateRecipeAsync(Guid userId, CreateRecipeDto request)
    {
        try
        {
            var validationResult = ValidateCreateRecipeRequest(request);
            if (validationResult.IsFailed)
                return RecipeResult.Failure(validationResult.Errors);

            if (!await _kitchenRepository.UserOwnsKitchenAsync(request.KitchenId, userId))
                return RecipeResult.Failure(RecipeErrors.Authorization.KitchenAccessDenied(request.KitchenId, userId));

            if (await _recipeRepository.ExistsInKitchenAsync(request.Name, request.KitchenId))
                return RecipeResult.Failure(RecipeErrors.BusinessRules.AlreadyExistsInKitchen(request.Name, request.KitchenId));

            var ingredientValidation = await ValidateRecipeIngredientsAsync(request.Ingredients, request.KitchenId);
            if (ingredientValidation.IsFailed)
                return RecipeResult.Failure(ingredientValidation.Errors);

            var recipe = new RecipeEntity
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                Yield = request.Yield,
                PrepTimeMinutes = request.PrepTimeMinutes,
                Instructions = request.Instructions?.Trim() ?? string.Empty,
                SellingPrice = request.SellingPrice,
                KitchenId = request.KitchenId,
                CreatedAt = DateTime.UtcNow,
                RecipeIngredients = new List<RecipeIngredientEntity>()
            };

            foreach (var ingredientReq in request.Ingredients)
            {
                var recipeIngredient = new RecipeIngredientEntity
                {
                    IngredientId = ingredientReq.IngredientId,
                    Quantity = ingredientReq.Quantity,
                    Notes = ingredientReq.Notes?.Trim() ?? string.Empty
                };

                recipe.RecipeIngredients.Add(recipeIngredient);
            }

            var createdRecipe = await _recipeRepository.CreateAsync(recipe);

            var recipeWithDetails = await _recipeRepository.GetByIdWithIngredientsAsync(createdRecipe.Id);
            var response = await MapToResponseAsync(recipeWithDetails!);

            return RecipeResult.Success(response);
        }
        catch (Exception ex)
        {
            return RecipeResult.Failure(RecipeErrors.UnexpectedError("recipe creation", ex));
        }
    }

    public async Task<RecipeResult> GetRecipeByIdAsync(Guid id, Guid userId)
    {
        try
        {
            var recipe = await _recipeRepository.GetByIdWithIngredientsAsync(id);
            if (recipe == null)
                return RecipeResult.Failure(RecipeErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(recipe.KitchenId, userId))
                return RecipeResult.Failure(RecipeErrors.Authorization.KitchenAccessDenied(recipe.KitchenId, userId));

            var response = await MapToResponseAsync(recipe);
            return RecipeResult.Success(response);
        }
        catch (Exception ex)
        {
            return RecipeResult.Failure(RecipeErrors.UnexpectedError("get recipe by id", ex));
        }
    }

    public async Task<RecipeListResult> GetKitchenRecipesAsync(Guid kitchenId, Guid userId)
    {
        try
        {
            if (!await _kitchenRepository.UserOwnsKitchenAsync(kitchenId, userId))
                return RecipeListResult.Failure(RecipeErrors.Authorization.KitchenAccessDenied(kitchenId, userId));

            var recipes = await _recipeRepository.GetByKitchenIdAsync(kitchenId);
            var responses = new List<RecipeResponseDto>();

            foreach (var recipe in recipes)
            {
                responses.Add(await MapToResponseAsync(recipe));
            }

            return RecipeListResult.Success(responses);
        }
        catch (Exception ex)
        {
            return RecipeListResult.Failure(RecipeErrors.UnexpectedError("get kitchen recipes", ex));
        }
    }

    public async Task<RecipeResult> UpdateRecipeAsync(Guid id, Guid userId, UpdateRecipeDto request)
    {
        try
        {
            var validationResult = ValidateUpdateRecipeRequest(request);
            if (validationResult.IsFailed)
                return RecipeResult.Failure(validationResult.Errors);

            var recipe = await _recipeRepository.GetByIdWithIngredientsAsync(id);
            if (recipe == null)
                return RecipeResult.Failure(RecipeErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(recipe.KitchenId, userId))
                return RecipeResult.Failure(RecipeErrors.Authorization.KitchenAccessDenied(recipe.KitchenId, userId));

            if (await _recipeRepository.ExistsInKitchenAsync(request.Name, recipe.KitchenId, id))
                return RecipeResult.Failure(RecipeErrors.BusinessRules.AlreadyExistsInKitchen(request.Name, recipe.KitchenId));

            var ingredientValidation = await ValidateRecipeIngredientsAsync(request.Ingredients, recipe.KitchenId);
            if (ingredientValidation.IsFailed)
                return RecipeResult.Failure(ingredientValidation.Errors);

            recipe.Name = request.Name.Trim();
            recipe.Description = request.Description?.Trim() ?? string.Empty;
            recipe.Yield = request.Yield;
            recipe.PrepTimeMinutes = request.PrepTimeMinutes;
            recipe.Instructions = request.Instructions?.Trim() ?? string.Empty;
            recipe.SellingPrice = request.SellingPrice;

            recipe.RecipeIngredients.Clear();

            foreach (var ingredientReq in request.Ingredients)
            {
                recipe.RecipeIngredients.Add(new RecipeIngredientEntity
                {
                    IngredientId = ingredientReq.IngredientId,
                    Quantity = ingredientReq.Quantity,
                    Notes = ingredientReq.Notes?.Trim() ?? string.Empty
                });
            }

            var updatedRecipe = await _recipeRepository.UpdateAsync(recipe);
            var recipeWithDetails = await _recipeRepository.GetByIdWithIngredientsAsync(updatedRecipe.Id);
            var response = await MapToResponseAsync(recipeWithDetails!);

            return RecipeResult.Success(response);
        }
        catch (Exception ex)
        {
            return RecipeResult.Failure(RecipeErrors.UnexpectedError("recipe update", ex));
        }
    }

    public async Task<RecipeResult> DeleteRecipeAsync(Guid id, Guid userId)
    {
        try
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            if (recipe == null)
                return RecipeResult.Failure(RecipeErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(recipe.KitchenId, userId))
                return RecipeResult.Failure(RecipeErrors.Authorization.KitchenAccessDenied(recipe.KitchenId, userId));

            var success = await _recipeRepository.DeleteAsync(id);
            if (!success)
                return RecipeResult.Failure(RecipeErrors.DatabaseError("recipe deletion"));

            return RecipeResult.Success();
        }
        catch (Exception ex)
        {
            return RecipeResult.Failure(RecipeErrors.UnexpectedError("recipe deletion", ex));
        }
    }

    public async Task<RecipeProductionResult> CalculateProductionCapacityAsync(Guid recipeId, Guid userId)
    {
        try
        {
            var recipe = await _recipeRepository.GetByIdWithIngredientsAsync(recipeId);
            if (recipe == null)
                return RecipeProductionResult.Failure(RecipeErrors.Authorization.NotFound(recipeId));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(recipe.KitchenId, userId))
                return RecipeProductionResult.Failure(RecipeErrors.Authorization.KitchenAccessDenied(recipe.KitchenId, userId));

            var ingredientStatus = new List<IngredientStockStatusDto>();
            decimal maxProduction = decimal.MaxValue;
            string limitingFactor = "";

            foreach (var recipeIngredient in recipe.RecipeIngredients)
            {
                var availableStock = recipeIngredient.Ingredient.CurrentStock;
                var requiredPerPortion = recipeIngredient.Quantity / recipe.Yield;
                var maxPortionsFromIngredient = requiredPerPortion > 0 ? availableStock / requiredPerPortion : 0;

                ingredientStatus.Add(new IngredientStockStatusDto(
                    recipeIngredient.IngredientId,
                    recipeIngredient.Ingredient.Name,
                    recipeIngredient.Quantity,
                    availableStock,
                    maxPortionsFromIngredient,
                    availableStock >= recipeIngredient.Quantity
                ));

                if (maxPortionsFromIngredient < maxProduction)
                {
                    maxProduction = maxPortionsFromIngredient;
                    limitingFactor = recipeIngredient.Ingredient.Name;
                }
            }

            var canProduce = maxProduction > 0;
            if (maxProduction == decimal.MaxValue) maxProduction = 0;

            var response = new RecipeProductionResponseDto(
                recipe.Id,
                recipe.Name,
                Math.Floor(maxProduction),
                ingredientStatus,
                canProduce,
                limitingFactor
            );

            return RecipeProductionResult.Success(response);
        }
        catch (Exception ex)
        {
            return RecipeProductionResult.Failure(RecipeErrors.UnexpectedError("calculate production capacity", ex));
        }
    }

    public async Task<RecipeResult> ProduceRecipeAsync(Guid recipeId, Guid userId, ProduceRecipeDto request)
    {
        try
        {
            var productionCapacity = await CalculateProductionCapacityAsync(recipeId, userId);
            if (productionCapacity.IsFailed)
                return RecipeResult.Failure(productionCapacity.Errors);

            if (request.Quantity > productionCapacity.Value.MaxProductionQuantity)
            {
                var missingIngredients = productionCapacity.Value.IngredientStatus
                    .Where(s => !s.IsSufficient)
                    .Select(s => s.IngredientName)
                    .ToList();

                return RecipeResult.Failure(RecipeErrors.BusinessRules.InsufficientStockForProduction(
                    productionCapacity.Value.RecipeName, missingIngredients));
            }

            var recipe = await _recipeRepository.GetByIdWithIngredientsAsync(recipeId);

            foreach (var recipeIngredient in recipe!.RecipeIngredients)
            {
                var totalQuantityNeeded = (recipeIngredient.Quantity / recipe.Yield) * request.Quantity;

                var stockResult = await _stockRepository.CreateAsync(new StockEntryEntity
                {
                    IngredientId = recipeIngredient.IngredientId,
                    MovementType = StockMovementType.Usage,
                    Quantity = -totalQuantityNeeded,
                    Reason = $"Production of {request.Quantity} portions of {recipe.Name}",
                    Reference = request.Notes?.Trim() ?? "",
                    MovementDate = DateTime.UtcNow
                });

                recipeIngredient.Ingredient.CurrentStock -= totalQuantityNeeded;
                await _ingredientRepository.UpdateAsync(recipeIngredient.Ingredient);
            }

            var updatedRecipe = await _recipeRepository.GetByIdWithIngredientsAsync(recipeId);
            var response = await MapToResponseAsync(updatedRecipe!);

            return RecipeResult.Success(response);
        }
        catch (Exception ex)
        {
            return RecipeResult.Failure(RecipeErrors.UnexpectedError("recipe production", ex));
        }
    }

    public async Task<RecipeCostAnalysisResult> GetCostAnalysisAsync(Guid recipeId, Guid userId)
    {
        try
        {
            var recipe = await _recipeRepository.GetByIdWithIngredientsAsync(recipeId);
            if (recipe == null)
                return RecipeCostAnalysisResult.Failure(RecipeErrors.Authorization.NotFound(recipeId));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(recipe.KitchenId, userId))
                return RecipeCostAnalysisResult.Failure(RecipeErrors.Authorization.KitchenAccessDenied(recipe.KitchenId, userId));

            var costBreakdown = new List<IngredientCostBreakdownDto>();
            decimal totalCost = 0;

            foreach (var recipeIngredient in recipe.RecipeIngredients)
            {
                var unitPrice = await _stockRepository.GetLastUnitPriceAsync(recipeIngredient.IngredientId) ?? 0;
                var ingredientTotalCost = recipeIngredient.Quantity * unitPrice;
                totalCost += ingredientTotalCost;

                costBreakdown.Add(new IngredientCostBreakdownDto(
                    recipeIngredient.Ingredient.Name,
                    recipeIngredient.Quantity,
                    recipeIngredient.Ingredient.UnitOfMeasure?.Symbol ?? "UN",
                    unitPrice,
                    ingredientTotalCost,
                    0 // Will be calculated after we have total
                ));
            }

            // Calculate percentages
            for (int i = 0; i < costBreakdown.Count; i++)
            {
                var percentage = totalCost > 0 ? (costBreakdown[i].TotalCost / totalCost) * 100 : 0;
                costBreakdown[i] = costBreakdown[i] with { PercentageOfTotal = percentage };
            }

            var costPerPortion = recipe.Yield > 0 ? totalCost / recipe.Yield : 0;
            decimal? profitPerPortion = recipe.SellingPrice.HasValue ? recipe.SellingPrice.Value - costPerPortion : null;
            decimal? profitMarginPercent = recipe.SellingPrice.HasValue && recipe.SellingPrice.Value > 0
                ? ((recipe.SellingPrice.Value - costPerPortion) / recipe.SellingPrice.Value) * 100
                : null;

            var analysis = new RecipeCostAnalysisResponseDto(
                recipe.Id,
                recipe.Name,
                totalCost,
                costPerPortion,
                recipe.SellingPrice,
                profitPerPortion,
                profitMarginPercent,
                costBreakdown
            );

            return RecipeCostAnalysisResult.Success(analysis);
        }
        catch (Exception ex)
        {
            return RecipeCostAnalysisResult.Failure(RecipeErrors.UnexpectedError("cost analysis", ex));
        }
    }

    private Result ValidateCreateRecipeRequest(CreateRecipeDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(RecipeErrors.Validation.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(RecipeErrors.Validation.NameTooLong(100));

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 1000)
            errors.Add(RecipeErrors.Validation.DescriptionTooLong(1000));

        if (request.Yield <= 0)
            errors.Add(RecipeErrors.Validation.InvalidYield);

        if (request.PrepTimeMinutes < 0)
            errors.Add(RecipeErrors.Validation.InvalidPrepTime);

        if (request.SellingPrice.HasValue && request.SellingPrice.Value <= 0)
            errors.Add(RecipeErrors.Validation.InvalidSellingPrice);

        if (!string.IsNullOrEmpty(request.Instructions) && request.Instructions.Length > 5000)
            errors.Add(RecipeErrors.Validation.InstructionsTooLong(5000));

        if (request.Ingredients == null || !request.Ingredients.Any())
            errors.Add(RecipeErrors.BusinessRules.NoIngredientsProvided);
        else
        {
            var ingredientIds = new HashSet<Guid>();
            foreach (var ingredient in request.Ingredients)
            {
                if (ingredient.Quantity <= 0)
                    errors.Add(RecipeErrors.Validation.InvalidIngredientQuantity(ingredient.Quantity));

                if (ingredientIds.Contains(ingredient.IngredientId))
                    errors.Add(RecipeErrors.BusinessRules.DuplicateIngredient(ingredient.IngredientId));
                else
                    ingredientIds.Add(ingredient.IngredientId);
            }
        }

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private Result ValidateUpdateRecipeRequest(UpdateRecipeDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(RecipeErrors.Validation.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(RecipeErrors.Validation.NameTooLong(100));

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 1000)
            errors.Add(RecipeErrors.Validation.DescriptionTooLong(1000));

        if (request.Yield <= 0)
            errors.Add(RecipeErrors.Validation.InvalidYield);

        if (request.PrepTimeMinutes < 0)
            errors.Add(RecipeErrors.Validation.InvalidPrepTime);

        if (request.SellingPrice.HasValue && request.SellingPrice.Value <= 0)
            errors.Add(RecipeErrors.Validation.InvalidSellingPrice);

        if (!string.IsNullOrEmpty(request.Instructions) && request.Instructions.Length > 5000)
            errors.Add(RecipeErrors.Validation.InstructionsTooLong(5000));

        if (request.Ingredients == null || !request.Ingredients.Any())
            errors.Add(RecipeErrors.BusinessRules.NoIngredientsProvided);
        else
        {
            var ingredientIds = new HashSet<Guid>();
            foreach (var ingredient in request.Ingredients)
            {
                if (ingredient.Quantity <= 0)
                    errors.Add(RecipeErrors.Validation.InvalidIngredientQuantity(ingredient.Quantity));

                if (ingredientIds.Contains(ingredient.IngredientId))
                    errors.Add(RecipeErrors.BusinessRules.DuplicateIngredient(ingredient.IngredientId));
                else
                    ingredientIds.Add(ingredient.IngredientId);
            }
        }

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private async Task<Result> ValidateRecipeIngredientsAsync(List<RecipeIngredientDto> ingredients, Guid kitchenId)
    {
        var errors = new List<IError>();
        var ingredientIds = ingredients.Select(i => i.IngredientId).ToArray();
        var existingIngredients = await _ingredientRepository.GetByIdsAsync(ingredientIds);

        foreach (var ingredientReq in ingredients)
        {
            var ingredient = existingIngredients.FirstOrDefault(i => i.Id == ingredientReq.IngredientId);
            if (ingredient == null)
            {
                errors.Add(IngredientErrors.Authorization.NotFound(ingredientReq.IngredientId));
                continue;
            }

            if (ingredient.KitchenId != kitchenId)
            {
                errors.Add(RecipeErrors.BusinessRules.IngredientNotInKitchen(ingredientReq.IngredientId, kitchenId));
            }
        }

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private async Task<RecipeResponseDto> MapToResponseAsync(RecipeEntity recipe)
    {
        var ingredients = new List<RecipeIngredientResponseDto>();

        foreach (var recipeIngredient in recipe.RecipeIngredients)
        {
            var unitPrice = await _stockRepository.GetLastUnitPriceAsync(recipeIngredient.IngredientId) ?? 0;
            var totalIngredientCost = recipeIngredient.Quantity * unitPrice;
            var hasSufficientStock = recipeIngredient.Ingredient.CurrentStock >= recipeIngredient.Quantity;

            ingredients.Add(new RecipeIngredientResponseDto(
                recipeIngredient.IngredientId,
                recipeIngredient.Ingredient.Name,
                recipeIngredient.Ingredient.UnitOfMeasure?.Symbol ?? "UN",
                recipeIngredient.Quantity,
                recipeIngredient.Notes,
                unitPrice,
                totalIngredientCost,
                hasSufficientStock,
                recipeIngredient.Ingredient.CurrentStock
            ));
        }

        var totalCost = ingredients.Sum(i => i.TotalCost);
        var costPerPortion = recipe.Yield > 0 ? totalCost / recipe.Yield : 0;
        decimal? profitMargin = recipe.SellingPrice.HasValue && recipe.SellingPrice.Value > 0
            ? ((recipe.SellingPrice.Value - costPerPortion) / recipe.SellingPrice.Value) * 100
            : null;

        return new RecipeResponseDto(
            recipe.Id,
            recipe.Name,
            recipe.Description,
            recipe.Yield,
            recipe.PrepTimeMinutes,
            recipe.Instructions,
            recipe.SellingPrice,
            totalCost,
            costPerPortion,
            profitMargin,
            ingredients,
            recipe.CreatedAt
        );
    }
}