using FluentResults;
using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Ingredients.Results;
using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Stock.Abstractions;
using KitchenStock.Application.Modules.Stock.Dtos;
using KitchenStock.Application.Modules.Stock.Results;
using KitchenStock.Domain.Entities;
using KitchenStock.Domain.Enums;

namespace KitchenStock.Infrastructure.Modules.Stock.Services;

public class StockEntryService : IStockEntryService
{
    private readonly IStockEntryRepository _stockRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IKitchenRepository _kitchenRepository;

    public StockEntryService(
        IStockEntryRepository stockRepository,
        IIngredientRepository ingredientRepository,
        IKitchenRepository kitchenRepository)
    {
        _stockRepository = stockRepository;
        _ingredientRepository = ingredientRepository;
        _kitchenRepository = kitchenRepository;
    }

    public async Task<StockEntryResult> AddStockEntryAsync(Guid userId, CreateStockEntryDto request)
    {
        try
        {
            var validationResult = ValidateCreateStockEntryRequest(request);
            if (validationResult.IsFailed)
                return StockEntryResult.Failure(validationResult.Errors);

            var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId);
            if (ingredient == null)
                return StockEntryResult.Failure(IngredientErrors.Authorization.NotFound(request.IngredientId));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(ingredient.KitchenId, userId))
                return StockEntryResult.Failure(StockErrors.Authorization.IngredientAccessDenied(request.IngredientId, userId));

            var stockResult = await ProcessStockMovementAsync(request.IngredientId, request.Quantity, request.MovementType, request.Reason);
            if (stockResult.IsFailed)
                return StockEntryResult.Failure(stockResult.Errors);

            var stockEntry = new StockEntryEntity
            {
                IngredientId = request.IngredientId,
                MovementType = request.MovementType,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                SupplierId = request.SupplierId,
                Reason = request.Reason?.Trim() ?? string.Empty,
                Reference = request.Reference?.Trim() ?? string.Empty,
                MovementDate = DateTime.UtcNow
            };

            var createdEntry = await _stockRepository.CreateAsync(stockEntry);
            var entryWithDetails = await _stockRepository.GetByIdAsync(createdEntry.Id);
            var response = MapToResponse(entryWithDetails!);

            return StockEntryResult.Success(response);
        }
        catch (Exception ex)
        {
            return StockEntryResult.Failure(StockErrors.UnexpectedError("add stock entry", ex));
        }
    }

    public async Task<StockEntryResult> GetStockEntryByIdAsync(Guid id, Guid userId)
    {
        try
        {
            var stockEntry = await _stockRepository.GetByIdAsync(id);
            if (stockEntry == null)
                return StockEntryResult.Failure(StockErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(stockEntry.Ingredient.KitchenId, userId))
                return StockEntryResult.Failure(StockErrors.Authorization.IngredientAccessDenied(stockEntry.IngredientId, userId));

            var response = MapToResponse(stockEntry);
            return StockEntryResult.Success(response);
        }
        catch (Exception ex)
        {
            return StockEntryResult.Failure(StockErrors.UnexpectedError("get stock entry", ex));
        }
    }

    public async Task<StockEntryListResult> GetIngredientStockHistoryAsync(Guid ingredientId, Guid userId)
    {
        try
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(ingredientId);
            if (ingredient == null)
                return StockEntryListResult.Failure(IngredientErrors.Authorization.NotFound(ingredientId));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(ingredient.KitchenId, userId))
                return StockEntryListResult.Failure(StockErrors.Authorization.IngredientAccessDenied(ingredientId, userId));

            var stockEntries = await _stockRepository.GetByIngredientIdAsync(ingredientId);
            var responses = stockEntries.Select(MapToResponse).ToList();

            return StockEntryListResult.Success(responses);
        }
        catch (Exception ex)
        {
            return StockEntryListResult.Failure(StockErrors.UnexpectedError("get ingredient stock history", ex));
        }
    }

    public async Task<StockEntryListResult> GetKitchenStockHistoryAsync(Guid kitchenId, Guid userId, DateTime? fromDate = null)
    {
        try
        {
            if (!await _kitchenRepository.UserOwnsKitchenAsync(kitchenId, userId))
                return StockEntryListResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(kitchenId, userId));

            var stockEntries = await _stockRepository.GetByKitchenIdAsync(kitchenId, fromDate);
            var responses = stockEntries.Select(MapToResponse).ToList();

            return StockEntryListResult.Success(responses);
        }
        catch (Exception ex)
        {
            return StockEntryListResult.Failure(StockErrors.UnexpectedError("get kitchen stock history", ex));
        }
    }

    public async Task<StockSummaryResult> GetStockSummaryAsync(Guid kitchenId, Guid userId)
    {
        try
        {
            if (!await _kitchenRepository.UserOwnsKitchenAsync(kitchenId, userId))
                return StockSummaryResult.Failure(IngredientErrors.Authorization.KitchenAccessDenied(kitchenId, userId));

            var ingredients = await _ingredientRepository.GetByKitchenIdAsync(kitchenId);
            var lowStockIngredients = await _ingredientRepository.GetLowStockByKitchenIdAsync(kitchenId);

            decimal totalValue = 0;
            var lowStockItems = new List<StockIngredientResponseDto>();

            foreach (var ingredient in ingredients)
            {
                var lastPrice = await _stockRepository.GetLastUnitPriceAsync(ingredient.Id);
                totalValue += ingredient.CurrentStock * (lastPrice ?? 0);
            }

            foreach (var ingredient in lowStockIngredients)
            {
                var lastPrice = await _stockRepository.GetLastUnitPriceAsync(ingredient.Id);
                var avgPrice = await _stockRepository.GetAverageUnitPriceAsync(ingredient.Id);

                lowStockItems.Add(new StockIngredientResponseDto(
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
                ));
            }

            var lastMonth = DateTime.UtcNow.AddDays(-30);
            var recentMovements = await _stockRepository.GetByKitchenIdAsync(kitchenId, lastMonth);

            var summary = new StockSummaryResponseDto(
                totalValue,
                ingredients.Count(),
                lowStockIngredients.Count(),
                recentMovements.Count(),
                lowStockItems
            );

            return StockSummaryResult.Success(summary);
        }
        catch (Exception ex)
        {
            return StockSummaryResult.Failure(StockErrors.CalculationError("stock summary", ex));
        }
    }

    public async Task<Result<bool>> ProcessStockMovementAsync(Guid ingredientId, decimal quantity, StockMovementType type, string reason, Guid? supplierId = null)
    {
        try
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(ingredientId);
            if (ingredient == null)
                return Result.Fail(IngredientErrors.Authorization.NotFound(ingredientId));

            var newStock = type switch
            {
                StockMovementType.Purchase => ingredient.CurrentStock + Math.Abs(quantity),
                StockMovementType.Usage => ingredient.CurrentStock - Math.Abs(quantity),
                StockMovementType.Waste => ingredient.CurrentStock - Math.Abs(quantity),
                StockMovementType.Return => ingredient.CurrentStock + Math.Abs(quantity),
                StockMovementType.Adjustment => quantity,
                _ => ingredient.CurrentStock
            };

            if (newStock < 0)
                return Result.Fail(StockErrors.BusinessRules.CannotReduceStockBelowZero(ingredient.Name, ingredient.CurrentStock, Math.Abs(quantity)));

            ingredient.CurrentStock = newStock;
            await _ingredientRepository.UpdateAsync(ingredient);

            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            return Result.Fail(StockErrors.DatabaseError("process stock movement", ex));
        }
    }

    private Result ValidateCreateStockEntryRequest(CreateStockEntryDto request)
    {
        var errors = new List<IError>();

        if (request.Quantity == 0)
            errors.Add(StockErrors.Validation.QuantityZero);

        if (request.MovementType == StockMovementType.Purchase && !request.UnitPrice.HasValue)
            errors.Add(StockErrors.Validation.UnitPriceRequiredForPurchase);

        if (request.UnitPrice.HasValue && request.UnitPrice.Value <= 0)
            errors.Add(StockErrors.Validation.InvalidUnitPrice(request.UnitPrice.Value));

        if (!string.IsNullOrEmpty(request.Reason) && request.Reason.Length > 200)
            errors.Add(StockErrors.Validation.ReasonTooLong(200));

        if (!string.IsNullOrEmpty(request.Reference) && request.Reference.Length > 50)
            errors.Add(StockErrors.Validation.ReferenceTooLong(50));

        if (!Enum.IsDefined(typeof(StockMovementType), request.MovementType))
            errors.Add(StockErrors.Validation.InvalidMovementType(request.MovementType.ToString()));

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private StockEntryResponseDto MapToResponse(StockEntryEntity stockEntry)
    {
        return new StockEntryResponseDto(
            stockEntry.Id,
            stockEntry.Ingredient?.Name ?? "N/A",
            stockEntry.MovementType,
            stockEntry.Quantity,
            stockEntry.UnitPrice,
            stockEntry.TotalValue,
            stockEntry.Supplier?.Name ?? "N/A",
            stockEntry.Reason,
            stockEntry.Reference,
            stockEntry.MovementDate
        );
    }
}
