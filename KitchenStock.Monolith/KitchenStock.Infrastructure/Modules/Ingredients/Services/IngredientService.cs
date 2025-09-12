using FluentResults;
using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Ingredients.Dtos;
using KitchenStock.Application.Modules.Ingredients.Results;
using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Stock.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Domain.Enums;

namespace KitchenStock.Infrastructure.Modules.Ingredients.Services;

public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IKitchenRepository _kitchenRepository;
    private readonly IStockEntryRepository _stockRepository;

    public IngredientService(
        IIngredientRepository ingredientRepository,
        IKitchenRepository kitchenRepository,
        IStockEntryRepository stockRepository)
    {
        _ingredientRepository = ingredientRepository;
        _kitchenRepository = kitchenRepository;
        _stockRepository = stockRepository;
    }

    public async Task<IngredientResult> CreateIngredientAsync(Guid userId, CreateIngredientDto request)
    {
        try
        {
            var validationResult = ValidateCreateIngredientRequest(request);
            if (validationResult.IsFailed)
                return IngredientResult.Failure(validationResult.Errors);

            if (!await _kitchenRepository.UserOwnsKitchenAsync(request.KitchenId, userId))
                return IngredientResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(request.KitchenId, userId));

            if (await _ingredientRepository.ExistsInKitchenAsync(request.Name, request.KitchenId))
                return IngredientResult.Failure(IngredientErrors.BusinessRules.AlreadyExistsInKitchen(request.Name, request.KitchenId));

            var ingredient = new IngredientEntity
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                KitchenId = request.KitchenId,
                UnitOfMeasureId = request.UnitOfMeasureId,
                CurrentStock = 0,
                MinimumStock = request.MinimumStock
            };

            var createdIngredient = await _ingredientRepository.CreateAsync(ingredient);
            var ingredientWithDetails = await _ingredientRepository.GetByIdAsync(createdIngredient.Id);
            var response = await MapToResponseAsync(ingredientWithDetails!);

            return IngredientResult.Success(response);
        }
        catch (Exception ex)
        {
            return IngredientResult.Failure(IngredientErrors.UnexpectedError("ingredient creation", ex));
        }
    }

    public async Task<IngredientResult> GetIngredientByIdAsync(Guid id, Guid userId)
    {
        try
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
                return IngredientResult.Failure(IngredientErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(ingredient.KitchenId, userId))
                return IngredientResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(ingredient.KitchenId, userId));

            var response = await MapToResponseAsync(ingredient);
            return IngredientResult.Success(response);
        }
        catch (Exception ex)
        {
            return IngredientResult.Failure(IngredientErrors.UnexpectedError("get ingredient by id", ex));
        }
    }

    public async Task<IngredientListResult> GetKitchenIngredientsAsync(Guid kitchenId, Guid userId)
    {
        try
        {
            if (!await _kitchenRepository.UserOwnsKitchenAsync(kitchenId, userId))
                return IngredientListResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(kitchenId, userId));

            var ingredients = await _ingredientRepository.GetByKitchenIdAsync(kitchenId);
            var responses = new List<IngredientResponseDto>();

            foreach (var ingredient in ingredients)
            {
                responses.Add(await MapToResponseAsync(ingredient));
            }

            return IngredientListResult.Success(responses);
        }
        catch (Exception ex)
        {
            return IngredientListResult.Failure(IngredientErrors.UnexpectedError("get kitchen ingredients", ex));
        }
    }

    public async Task<IngredientListResult> GetLowStockIngredientsAsync(Guid kitchenId, Guid userId)
    {
        try
        {
            if (!await _kitchenRepository.UserOwnsKitchenAsync(kitchenId, userId))
                return IngredientListResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(kitchenId, userId));

            var ingredients = await _ingredientRepository.GetLowStockByKitchenIdAsync(kitchenId);
            var responses = new List<IngredientResponseDto>();

            foreach (var ingredient in ingredients)
            {
                responses.Add(await MapToResponseAsync(ingredient));
            }

            return IngredientListResult.Success(responses);
        }
        catch (Exception ex)
        {
            return IngredientListResult.Failure(IngredientErrors.UnexpectedError("get low stock ingredients", ex));
        }
    }

    public async Task<IngredientResult> UpdateIngredientAsync(Guid id, Guid userId, UpdateIngredientDto request)
    {
        try
        {
            var validationResult = ValidateUpdateIngredientRequest(request);
            if (validationResult.IsFailed)
                return IngredientResult.Failure(validationResult.Errors);

            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
                return IngredientResult.Failure(IngredientErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(ingredient.KitchenId, userId))
                return IngredientResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(ingredient.KitchenId, userId));

            if (await _ingredientRepository.ExistsInKitchenAsync(request.Name, ingredient.KitchenId, id))
                return IngredientResult.Failure(IngredientErrors.BusinessRules.AlreadyExistsInKitchen(request.Name, ingredient.KitchenId));

            ingredient.Name = request.Name.Trim();
            ingredient.Description = request.Description?.Trim() ?? string.Empty;
            ingredient.UnitOfMeasureId = request.UnitOfMeasureId;
            ingredient.MinimumStock = request.MinimumStock;

            var updatedIngredient = await _ingredientRepository.UpdateAsync(ingredient);
            var ingredientWithDetails = await _ingredientRepository.GetByIdAsync(updatedIngredient.Id);
            var response = await MapToResponseAsync(ingredientWithDetails!);

            return IngredientResult.Success(response);
        }
        catch (Exception ex)
        {
            return IngredientResult.Failure(IngredientErrors.UnexpectedError("ingredient update", ex));
        }
    }

    public async Task<IngredientResult> DeleteIngredientAsync(Guid id, Guid userId)
    {
        try
        {
            var ingredient = await _ingredientRepository.GetByIdWithStockHistoryAsync(id);
            if (ingredient == null)
                return IngredientResult.Failure(IngredientErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(ingredient.KitchenId, userId))
                return IngredientResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(ingredient.KitchenId, userId));

            if (ingredient.StockEntries?.Any() == true)
                return IngredientResult.Failure(IngredientErrors.BusinessRules.CannotDeleteWithStockHistory(ingredient.Name, ingredient.StockEntries.Count));

            if (ingredient.RecipeIngredients?.Any() == true)
                return IngredientResult.Failure(IngredientErrors.BusinessRules.CannotDeleteWithActiveRecipes(ingredient.Name, ingredient.RecipeIngredients.Count));

            var success = await _ingredientRepository.DeleteAsync(id);
            if (!success)
                return IngredientResult.Failure(IngredientErrors.DatabaseError("ingredient deletion"));

            return IngredientResult.Success();
        }
        catch (Exception ex)
        {
            return IngredientResult.Failure(IngredientErrors.UnexpectedError("ingredient deletion", ex));
        }
    }

    public async Task<IngredientResult> UpdateStockAsync(Guid id, Guid userId, UpdateStockDto request)
    {
        try
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
                return IngredientResult.Failure(IngredientErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(ingredient.KitchenId, userId))
                return IngredientResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(ingredient.KitchenId, userId));

            if (request.NewStock < 0)
                return IngredientResult.Failure(IngredientErrors.Validation.InvalidCurrentStock);

            var oldStock = ingredient.CurrentStock;
            ingredient.CurrentStock = request.NewStock;

            var updatedIngredient = await _ingredientRepository.UpdateAsync(ingredient);

            var stockEntry = new StockEntryEntity
            {
                IngredientId = id,
                MovementType = StockMovementType.Adjustment,
                Quantity = request.NewStock - oldStock,
                Reason = request.Reason?.Trim() ?? "Stock adjustment",
                MovementDate = DateTime.UtcNow
            };

            await _stockRepository.CreateAsync(stockEntry);

            var response = await MapToResponseAsync(updatedIngredient);
            return IngredientResult.Success(response);
        }
        catch (Exception ex)
        {
            return IngredientResult.Failure(IngredientErrors.UnexpectedError("stock update", ex));
        }
    }

    private Result ValidateCreateIngredientRequest(CreateIngredientDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(IngredientErrors.Validation.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(IngredientErrors.Validation.NameTooLong(100));

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 500)
            errors.Add(IngredientErrors.Validation.DescriptionTooLong(500));

        if (request.MinimumStock < 0)
            errors.Add(IngredientErrors.Validation.InvalidMinimumStock);

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private Result ValidateUpdateIngredientRequest(UpdateIngredientDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(IngredientErrors.Validation.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(IngredientErrors.Validation.NameTooLong(100));

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 500)
            errors.Add(IngredientErrors.Validation.DescriptionTooLong(500));

        if (request.MinimumStock < 0)
            errors.Add(IngredientErrors.Validation.InvalidMinimumStock);

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private async Task<IngredientResponseDto> MapToResponseAsync(IngredientEntity ingredient)
    {
        var lastPrice = await _stockRepository.GetLastUnitPriceAsync(ingredient.Id);
        var avgPrice = await _stockRepository.GetAverageUnitPriceAsync(ingredient.Id);

        return new IngredientResponseDto(
            ingredient.Id,
            ingredient.Name,
            ingredient.Description,
            ingredient.UnitOfMeasure?.Name ?? "N/A",
            ingredient.UnitOfMeasure?.Symbol ?? "N/A",
            ingredient.CurrentStock,
            ingredient.MinimumStock,
            ingredient.IsLowStock,
            lastPrice ?? 0,
            avgPrice,
            ingredient.CreatedAt
        );
    }
}
