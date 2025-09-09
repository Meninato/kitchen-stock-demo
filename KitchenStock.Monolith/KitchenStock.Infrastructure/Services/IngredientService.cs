using FluentResults;
using KitchenStock.Application.Abstractions;
using KitchenStock.Application.Errors;
using KitchenStock.Domain.Enums;

namespace KitchenStock.Infrastructure.Services;

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

    public async Task<IngredientResult> CreateIngredientAsync(int userId, CreateIngredientRequest request)
    {
        try
        {
            // Validação de entrada
            var validationResult = ValidateCreateIngredientRequest(request);
            if (validationResult.IsFailed)
                return IngredientResult.Failure(validationResult.Errors);

            // Verificar se usuário tem acesso à cozinha
            if (!await _kitchenRepository.UserOwnsKitchenAsync(request.KitchenId, userId))
                return IngredientResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(request.KitchenId, userId));

            // Verificar se ingrediente já existe na cozinha
            if (await _ingredientRepository.ExistsInKitchenAsync(request.Name, request.KitchenId))
                return IngredientResult.Failure(IngredientErrors.BusinessRules.AlreadyExistsInKitchen(request.Name, request.KitchenId));

            // Criar ingrediente
            var ingredient = new Ingredient
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                Category = request.Category?.Trim() ?? string.Empty,
                KitchenId = request.KitchenId,
                UnitOfMeasureId = request.UnitOfMeasureId,
                CurrentStock = 0,
                MinimumStock = request.MinimumStock,
                CreatedAt = DateTime.UtcNow
            };

            var createdIngredient = await _ingredientRepository.CreateAsync(ingredient);

            // Recarregar com relacionamentos
            var ingredientWithDetails = await _ingredientRepository.GetByIdAsync(createdIngredient.Id);
            var response = await MapToResponseAsync(ingredientWithDetails!);

            return IngredientResult.Success(response);
        }
        catch (Exception ex)
        {
            return IngredientResult.Failure(IngredientErrors.UnexpectedError("ingredient creation", ex));
        }
    }

    public async Task<IngredientResult> GetIngredientByIdAsync(int id, int userId)
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

    public async Task<IngredientListResult> GetKitchenIngredientsAsync(int kitchenId, int userId)
    {
        try
        {
            if (!await _kitchenRepository.UserOwnsKitchenAsync(kitchenId, userId))
                return IngredientListResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(kitchenId, userId));

            var ingredients = await _ingredientRepository.GetByKitchenIdAsync(kitchenId);
            var responses = new List<IngredientResponse>();

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

    public async Task<IngredientListResult> GetLowStockIngredientsAsync(int kitchenId, int userId)
    {
        try
        {
            if (!await _kitchenRepository.UserOwnsKitchenAsync(kitchenId, userId))
                return IngredientListResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(kitchenId, userId));

            var ingredients = await _ingredientRepository.GetLowStockByKitchenIdAsync(kitchenId);
            var responses = new List<IngredientResponse>();

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

    public async Task<IngredientResult> UpdateIngredientAsync(int id, int userId, UpdateIngredientRequest request)
    {
        try
        {
            // Validação de entrada
            var validationResult = ValidateUpdateIngredientRequest(request);
            if (validationResult.IsFailed)
                return IngredientResult.Failure(validationResult.Errors);

            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
                return IngredientResult.Failure(IngredientErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(ingredient.KitchenId, userId))
                return IngredientResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(ingredient.KitchenId, userId));

            // Verificar se nome não conflita
            if (await _ingredientRepository.ExistsInKitchenAsync(request.Name, ingredient.KitchenId, id))
                return IngredientResult.Failure(IngredientErrors.BusinessRules.AlreadyExistsInKitchen(request.Name, ingredient.KitchenId));

            // Atualizar
            ingredient.Name = request.Name.Trim();
            ingredient.Description = request.Description?.Trim() ?? string.Empty;
            ingredient.Category = request.Category?.Trim() ?? string.Empty;
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

    public async Task<IngredientResult> DeleteIngredientAsync(int id, int userId)
    {
        try
        {
            var ingredient = await _ingredientRepository.GetByIdWithStockHistoryAsync(id);
            if (ingredient == null)
                return IngredientResult.Failure(IngredientErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(ingredient.KitchenId, userId))
                return IngredientResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(ingredient.KitchenId, userId));

            // Verificar regras de negócio
            if (ingredient.StockEntries?.Any() == true)
                return IngredientResult.Failure(IngredientErrors.BusinessRules.CannotDeleteWithStockHistory(ingredient.Name, ingredient.StockEntries.Count));

            if (ingredient.RecipeIngredients?.Any() == true)
                return IngredientResult.Failure(IngredientErrors.BusinessRules.CannotDeleteWithActiveRecipes(ingredient.Name, ingredient.RecipeIngredients.Count));

            var success = await _ingredientRepository.DeleteAsync(id);
            if (!success)
                return IngredientResult.Failure(IngredientErrors.DatabaseError("ingredient deletion"));

            var emptyResponse = new IngredientResponse(id, "", "", "", "", "", 0, 0, false, 0, 0, DateTime.MinValue);
            return IngredientResult.Success(emptyResponse);
        }
        catch (Exception ex)
        {
            return IngredientResult.Failure(IngredientErrors.UnexpectedError("ingredient deletion", ex));
        }
    }

    public async Task<IngredientResult> UpdateStockAsync(int id, int userId, UpdateStockRequest request)
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

            // Criar entrada de ajuste
            var stockEntry = new StockEntry
            {
                IngredientId = id,
                MovementType = StockMovementType.Adjustment,
                Quantity = request.NewStock - oldStock,
                Reason = request.Reason?.Trim() ?? "Stock adjustment",
                MovementDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
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

    private Result ValidateCreateIngredientRequest(CreateIngredientRequest request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(IngredientErrors.Validation.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(IngredientErrors.Validation.NameTooLong(100));

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 500)
            errors.Add(IngredientErrors.Validation.DescriptionTooLong(500));

        if (!string.IsNullOrEmpty(request.Category) && request.Category.Length > 50)
            errors.Add(IngredientErrors.Validation.CategoryTooLong(50));

        if (request.MinimumStock < 0)
            errors.Add(IngredientErrors.Validation.InvalidMinimumStock);

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private Result ValidateUpdateIngredientRequest(UpdateIngredientRequest request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(IngredientErrors.Validation.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(IngredientErrors.Validation.NameTooLong(100));

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 500)
            errors.Add(IngredientErrors.Validation.DescriptionTooLong(500));

        if (!string.IsNullOrEmpty(request.Category) && request.Category.Length > 50)
            errors.Add(IngredientErrors.Validation.CategoryTooLong(50));

        if (request.MinimumStock < 0)
            errors.Add(IngredientErrors.Validation.InvalidMinimumStock);

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private async Task<IngredientResponse> MapToResponseAsync(Ingredient ingredient)
    {
        var lastPrice = await _stockRepository.GetLastUnitPriceAsync(ingredient.Id);
        var avgPrice = await _stockRepository.GetAverageUnitPriceAsync(ingredient.Id);

        return new IngredientResponse(
            ingredient.Id,
            ingredient.Name,
            ingredient.Description,
            ingredient.Category,
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
