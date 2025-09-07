using FluentResults;
using Kitchen.Application.Dtos;
using Kitchen.Application.Errors;
using Kitchen.Application.Results;
using Kitchen.Application.Services.Abstractions;
using Kitchen.Domain.Entities;

namespace Kitchen.Infrastructure.Services;

public class KitchenService : IKitchenService
{
    private readonly IKitchenRepository _kitchenRepository;
    private readonly IUserApiClient _userApiClient;

    public KitchenService(IKitchenRepository kitchenRepository, IUserApiClient userApiClient)
    {
        _kitchenRepository = kitchenRepository;
        _userApiClient = userApiClient;
    }

    public async Task<KitchenResult> CreateKitchenAsync(Guid userId, CreateKitchenDto dto)
    {
        try
        {
            var validationResult = ValidateKitchenData(dto.Name);
            if (validationResult.IsFailed)
                return KitchenResult.Failure(validationResult.Errors);

            var userValidation = await ValidateUserCanCreateKitchen(userId);
            if (userValidation.IsFailed)
                return KitchenResult.Failure(userValidation.Errors);

            var userInfo = userValidation.Value;

            var currentKitchenCount = await _kitchenRepository.CountByOwnerIdAsync(userId);
            if (currentKitchenCount >= userInfo.MaxKitchens)
            {
                return KitchenResult.Failure(KitchenErrors.Creation.MaxKitchensExceeded(
                    userInfo.MaxKitchens, currentKitchenCount));
            }

            // Verificar feature premium
            if (dto.AllowSharedIngredients)
            {
                var featureValidation = await _userApiClient.ValidateUserPlanAsync(userId, "SharedIngredients");
                if (featureValidation.IsFailed || !featureValidation.Value)
                {
                    return KitchenResult.Failure(KitchenErrors.Creation.SharedIngredientsNotAllowed);
                }
            }

            // Criar entidade
            var kitchen = new KitchenEntity
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                Type = dto.Type,
                OwnerId = userId,
                Location = dto.Location?.Trim(),
                Phone = dto.Phone?.Trim(),
                AllowSharedIngredients = dto.AllowSharedIngredients,
                IsActive = true,
                LastActivityAt = DateTime.UtcNow
            };

            var createdKitchen = await _kitchenRepository.CreateAsync(kitchen);
            var responseDto = MapToResponseDto(createdKitchen);

            return KitchenResult.Success(responseDto);
        }
        catch (Exception ex)
        {
            return KitchenResult.Failure(KitchenErrors.UnexpectedError("kitchen creation", ex));
        }
    }

    public async Task<KitchenResult> UpdateKitchenAsync(Guid userId, Guid kitchenId, UpdateKitchenDto dto)
    {
        try
        {
            // Validar dados básicos
            var validationResult = ValidateKitchenData(dto.Name, dto.Type);
            if (validationResult.IsFailed)
                return KitchenResult.Failure(validationResult.Errors);

            // Verificar se a cozinha existe e pertence ao usuário
            var kitchen = await _kitchenRepository.GetByIdAndOwnerAsync(kitchenId, userId);
            if (kitchen == null)
            {
                return KitchenResult.Failure(KitchenErrors.Access.KitchenNotFound(kitchenId));
            }

            // Verificar feature premium se necessário
            if (dto.AllowSharedIngredients && !kitchen.AllowSharedIngredients)
            {
                var featureValidation = await _userApiClient.ValidateUserPlanAsync(userId, "SharedIngredients");
                if (featureValidation.IsFailed || !featureValidation.Value)
                {
                    return KitchenResult.Failure(KitchenErrors.Creation.SharedIngredientsNotAllowed);
                }
            }

            // Atualizar propriedades
            kitchen.Name = dto.Name.Trim();
            kitchen.Description = dto.Description?.Trim();
            kitchen.Type = dto.Type;
            kitchen.Location = dto.Location?.Trim();
            kitchen.Phone = dto.Phone?.Trim();
            kitchen.AllowSharedIngredients = dto.AllowSharedIngredients;
            kitchen.LastActivityAt = DateTime.UtcNow;

            var updatedKitchen = await _kitchenRepository.UpdateAsync(kitchen);
            var responseDto = MapToResponseDto(updatedKitchen);

            return KitchenResult.Success(responseDto);
        }
        catch (Exception ex)
        {
            return KitchenResult.Failure(KitchenErrors.UnexpectedError("kitchen update", ex));
        }
    }

    public async Task<KitchenResult> GetKitchenAsync(Guid userId, Guid kitchenId)
    {
        try
        {
            var kitchen = await _kitchenRepository.GetByIdAndOwnerAsync(kitchenId, userId);
            if (kitchen == null)
            {
                return KitchenResult.Failure(KitchenErrors.Access.KitchenNotFound(kitchenId));
            }

            var responseDto = MapToResponseDto(kitchen);
            return KitchenResult.Success(responseDto);
        }
        catch (Exception ex)
        {
            return KitchenResult.Failure(KitchenErrors.UnexpectedError("get kitchen", ex));
        }
    }

    public async Task<KitchenListResult> GetUserKitchensAsync(Guid userId, bool includeInactive = false)
    {
        try
        {
            var kitchens = await _kitchenRepository.GetByOwnerIdAsync(userId, includeInactive);
            var responseDtos = kitchens.Select(MapToResponseDto).ToList();

            return KitchenListResult.Success(responseDtos);
        }
        catch (Exception ex)
        {
            return KitchenListResult.Failure(KitchenErrors.UnexpectedError("get user kitchens", ex));
        }
    }

    public async Task<KitchenStatsResult> GetKitchenStatsAsync(Guid userId, Guid kitchenId)
    {
        try
        {
            var kitchen = await _kitchenRepository.GetByIdAndOwnerAsync(kitchenId, userId);
            if (kitchen == null)
            {
                return Result.Fail(KitchenErrors.Access.KitchenNotFound(kitchenId));
            }

            // TODO: Implementar cálculos reais quando tivermos Ingredient.Api e Recipe.Api
            var stats = new KitchenStatsResponseDto(
                KitchenId: kitchen.Id,
                KitchenName: kitchen.Name,
                TotalIngredients: kitchen.TotalIngredients,
                LowStockIngredients: 0, // Será calculado pelo Inventory.Api
                TotalRecipes: kitchen.TotalRecipes,
                TotalSuppliers: kitchen.TotalSuppliers,
                EstimatedInventoryValue: null, // Será calculado pelo Inventory.Api
                LastUpdated: DateTime.UtcNow
            );

            return Result.Ok(stats);
        }
        catch (Exception ex)
        {
            return Result.Fail(KitchenErrors.UnexpectedError("get kitchen stats", ex));
        }
    }

    public async Task<Result> DeleteKitchenAsync(Guid userId, Guid kitchenId)
    {
        try
        {
            var kitchen = await _kitchenRepository.GetByIdAndOwnerAsync(kitchenId, userId);
            if (kitchen == null)
            {
                return Result.Fail(KitchenErrors.Access.KitchenNotFound(kitchenId));
            }

            // TODO: Verificar se a cozinha tem ingredientes/receitas antes de deletar
            // Por enquanto, só fazemos soft delete

            var success = await _kitchenRepository.DeleteAsync(kitchenId);
            if (!success)
            {
                return Result.Fail(KitchenErrors.DatabaseError("kitchen deletion"));
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(KitchenErrors.UnexpectedError("kitchen deletion", ex));
        }
    }

    public async Task<Result> ActivateKitchenAsync(Guid userId, Guid kitchenId)
    {
        try
        {
            var kitchen = await _kitchenRepository.GetByIdAndOwnerAsync(kitchenId, userId);
            if (kitchen == null)
            {
                return Result.Fail(KitchenErrors.Access.KitchenNotFound(kitchenId));
            }

            kitchen.IsActive = true;
            kitchen.LastActivityAt = DateTime.UtcNow;

            await _kitchenRepository.UpdateAsync(kitchen);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(KitchenErrors.UnexpectedError("kitchen activation", ex));
        }
    }

    public async Task<Result> DeactivateKitchenAsync(Guid userId, Guid kitchenId)
    {
        try
        {
            var kitchen = await _kitchenRepository.GetByIdAndOwnerAsync(kitchenId, userId);
            if (kitchen == null)
            {
                return Result.Fail(KitchenErrors.Access.KitchenNotFound(kitchenId));
            }

            kitchen.IsActive = false;
            kitchen.LastActivityAt = DateTime.UtcNow;

            await _kitchenRepository.UpdateAsync(kitchen);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(KitchenErrors.UnexpectedError("kitchen deactivation", ex));
        }
    }

    #region Private Methods

    private Result ValidateKitchenData(string name)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(KitchenErrors.Creation.NameRequired);
        else if (name.Length > 100)
            errors.Add(KitchenErrors.Creation.NameTooLong(100));

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private async Task<Result<UserInfo>> ValidateUserCanCreateKitchen(Guid userId)
    {
        var userResult = await _userApiClient.GetUserAsync(userId);
        if (userResult.IsFailed)
        {
            return Result.Fail(KitchenErrors.Creation.UserNotFound(userId));
        }

        return Result.Ok(userResult.Value);
    }

    private KitchenResponseDto MapToResponseDto(KitchenEntity kitchen)
    {
        return new KitchenResponseDto(
            Id: kitchen.Id,
            Name: kitchen.Name,
            Description: kitchen.Description,
            Type: kitchen.Type,
            OwnerId: kitchen.OwnerId,
            AllowSharedIngredients: kitchen.AllowSharedIngredients,
            IsActive: kitchen.IsActive,
            Location: kitchen.Location,
            Phone: kitchen.Phone,
            TotalIngredients: kitchen.TotalIngredients,
            TotalRecipes: kitchen.TotalRecipes,
            TotalSuppliers: kitchen.TotalSuppliers,
            CreatedAt: kitchen.CreatedAt,
            LastActivityAt: kitchen.LastActivityAt
        );
    }

    #endregion
}
