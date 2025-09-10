using FluentResults;
using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Kitchen.Dtos;
using KitchenStock.Application.Modules.Kitchen.Results;
using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Application.Modules.User.Results;
using KitchenStock.Domain.Entities;
using KitchenStock.Domain.Enums;

namespace KitchenStock.Infrastructure.Modules.Kitchen.Services;

public class KitchenService : IKitchenService
{
    private readonly IKitchenRepository _kitchenRepository;
    private readonly IUserRepository _userRepository;

    public KitchenService(IKitchenRepository kitchenRepository, IUserRepository userRepository)
    {
        _kitchenRepository = kitchenRepository;
        _userRepository = userRepository;
    }

    public async Task<KitchenResult> CreateKitchenAsync(Guid userId, CreateKitchenDto request)
    {
        try
        {
            var validationResult = ValidateCreateKitchenRequest(request);
            if (validationResult.IsFailed)
                return KitchenResult.Failure(validationResult.Errors);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return KitchenResult.Failure(UserErrors.UserNotFound(userId));

            var currentKitchenCount = await _kitchenRepository.CountByUserIdAsync(userId);
            if (currentKitchenCount >= 5)
                return KitchenResult.Failure(KitchenErrors.BusinessRules.MaxKitchensReached(5, currentKitchenCount));

            var existingKitchens = await _kitchenRepository.GetByUserIdAsync(userId);
            if (existingKitchens.Any(k => k.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                return KitchenResult.Failure(KitchenErrors.BusinessRules.KitchenNameAlreadyExists(request.Name, userId));

            if (user.Plan == UserPlan.Basic && currentKitchenCount >= 1)
                return KitchenResult.Failure(KitchenErrors.BusinessRules.UserPlanDoesNotAllowMultipleKitchens(user.Plan.ToString()));

            var kitchen = new KitchenEntity
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                OwnerId = userId
            };

            var createdKitchen = await _kitchenRepository.CreateAsync(kitchen);
            var response = MapToResponse(createdKitchen);

            return KitchenResult.Success(response);
        }
        catch (Exception ex)
        {
            return KitchenResult.Failure(KitchenErrors.UnexpectedError("kitchen creation", ex));
        }
    }

    public async Task<KitchenResult> GetKitchenByIdAsync(Guid id, Guid userId)
    {
        try
        {
            var kitchen = await _kitchenRepository.GetByIdWithDetailsAsync(id);
            if (kitchen == null)
                return KitchenResult.Failure(KitchenErrors.Authorization.NotFound(id));

            if (kitchen.OwnerId != userId)
                return KitchenResult.Failure(KitchenErrors.Authorization.AccessDenied(id, userId));

            var response = MapToResponse(kitchen);
            return KitchenResult.Success(response);
        }
        catch (Exception ex)
        {
            return KitchenResult.Failure(KitchenErrors.UnexpectedError("get kitchen by id", ex));
        }
    }

    public async Task<KitchenListResult> GetUserKitchensAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return KitchenListResult.Failure(UserErrors.UserNotFound(userId));

            var kitchens = await _kitchenRepository.GetByUserIdAsync(userId);
            var responses = new List<KitchenResponseDto>();

            foreach (var kitchen in kitchens)
            {
                responses.Add(MapToResponse(kitchen));
            }

            return KitchenListResult.Success(responses);
        }
        catch (Exception ex)
        {
            return KitchenListResult.Failure(KitchenErrors.UnexpectedError("get user kitchens", ex));
        }
    }

    public async Task<KitchenResult> UpdateKitchenAsync(Guid id, Guid userId, UpdateKitchenDto request)
    {
        try
        {
            var validationResult = ValidateUpdateKitchenRequest(request);
            if (validationResult.IsFailed)
                return KitchenResult.Failure(validationResult.Errors);

            var kitchen = await _kitchenRepository.GetByIdAsync(id);
            if (kitchen == null)
                return KitchenResult.Failure(KitchenErrors.Authorization.NotFound(id));

            if (kitchen.OwnerId != userId)
                return KitchenResult.Failure(KitchenErrors.Authorization.AccessDenied(id, userId));

            var existingKitchens = await _kitchenRepository.GetByUserIdAsync(userId);
            if (existingKitchens.Any(k => k.Id != id && k.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                return KitchenResult.Failure(KitchenErrors.BusinessRules.KitchenNameAlreadyExists(request.Name, userId));

            kitchen.Name = request.Name.Trim();
            kitchen.Description = request.Description?.Trim() ?? string.Empty;

            var updatedKitchen = await _kitchenRepository.UpdateAsync(kitchen);
            var response = MapToResponse(updatedKitchen);

            return KitchenResult.Success(response);
        }
        catch (Exception ex)
        {
            return KitchenResult.Failure(KitchenErrors.UnexpectedError("kitchen update", ex));
        }
    }

    public async Task<KitchenResult> DeleteKitchenAsync(Guid id, Guid userId)
    {
        try
        {
            var kitchen = await _kitchenRepository.GetByIdWithDetailsAsync(id);
            if (kitchen == null)
                return KitchenResult.Failure(KitchenErrors.Authorization.NotFound(id));

            if (kitchen.OwnerId != userId)
                return KitchenResult.Failure(KitchenErrors.Authorization.AccessDenied(id, userId));

            if (kitchen.Ingredients?.Any() == true)
                return KitchenResult.Failure(KitchenErrors.BusinessRules.CannotDeleteWithActiveIngredients(kitchen.Ingredients.Count));

            if (kitchen.Recipes?.Any() == true)
                return KitchenResult.Failure(KitchenErrors.BusinessRules.CannotDeleteWithActiveRecipes(kitchen.Recipes.Count));

            var success = await _kitchenRepository.DeleteAsync(id);
            if (!success)
                return KitchenResult.Failure(KitchenErrors.DatabaseError("kitchen deletion"));

            return KitchenResult.Success();
        }
        catch (Exception ex)
        {
            return KitchenResult.Failure(KitchenErrors.UnexpectedError("kitchen deletion", ex));
        }
    }

    public async Task<KitchenStatsResult> GetKitchenStatsAsync(Guid kitchenId, Guid userId)
    {
        try
        {
            var kitchen = await _kitchenRepository.GetByIdWithDetailsAsync(kitchenId);
            if (kitchen == null)
                return KitchenStatsResult.Failure(KitchenErrors.Authorization.NotFound(kitchenId));

            if (kitchen.OwnerId != userId)
                return KitchenStatsResult.Failure(KitchenErrors.Authorization.AccessDenied(kitchenId, userId));

            var totalIngredients = kitchen.Ingredients?.Count ?? 0;
            var lowStockCount = kitchen.Ingredients?.Count(i => i.IsLowStock) ?? 0;
            var totalRecipes = kitchen.Recipes?.Count ?? 0;

            decimal totalStockValue = 0;
            if (kitchen.Ingredients?.Any() == true)
            {
                foreach (var ingredient in kitchen.Ingredients)
                {
                    totalStockValue += ingredient.CurrentStock * ingredient.LastUnitPrice;
                }
            }

            var lastMonth = DateTime.UtcNow.AddDays(-30);
            var recentMovements = 0;
            if (kitchen.Ingredients?.Any() == true)
            {
                recentMovements = kitchen.Ingredients
                    .SelectMany(i => i.StockEntries ?? new List<StockEntryEntity>())
                    .Count(se => se.MovementDate >= lastMonth);
            }

            var stats = new KitchenStatsResponseDto(
                totalIngredients,
                lowStockCount,
                totalRecipes,
                totalStockValue,
                recentMovements
            );

            return KitchenStatsResult.Success(stats);
        }
        catch (Exception ex)
        {
            return KitchenStatsResult.Failure(KitchenErrors.UnexpectedError("kitchen stats calculation", ex));
        }
    }

    private Result ValidateCreateKitchenRequest(CreateKitchenDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(KitchenErrors.Validation.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(KitchenErrors.Validation.NameTooLong(100));

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 500)
            errors.Add(KitchenErrors.Validation.DescriptionTooLong(500));

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private Result ValidateUpdateKitchenRequest(UpdateKitchenDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(KitchenErrors.Validation.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(KitchenErrors.Validation.NameTooLong(100));

        if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 500)
            errors.Add(KitchenErrors.Validation.DescriptionTooLong(500));

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private KitchenResponseDto MapToResponse(KitchenEntity kitchen)
    {
        var ingredientCount = kitchen.Ingredients?.Count ?? 0;
        var recipeCount = kitchen.Recipes?.Count ?? 0;
        var lowStockCount = kitchen.Ingredients?.Count(i => i.IsLowStock) ?? 0;

        return new KitchenResponseDto(
            kitchen.Id,
            kitchen.Name,
            kitchen.Description,
            ingredientCount,
            recipeCount,
            lowStockCount,
            kitchen.CreatedAt
        );
    }
}
