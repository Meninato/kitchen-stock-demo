using FluentResults;
using KitchenStock.Application.Modules.UnitOfMeasure.Abstractions;
using KitchenStock.Application.Modules.UnitOfMeasure.Dtos;
using KitchenStock.Application.Modules.UnitOfMeasure.Results;
using KitchenStock.Domain.Entities;
using System.Text.RegularExpressions;

namespace KitchenStock.Infrastructure.Modules.UnitOfMeasure.Services;

public class UnitOfMeasureService : IUnitOfMeasureService
{
    private readonly IUnitOfMeasureRepository _unitRepository;
    private static readonly string[] SystemUnits = { "KG", "G", "L", "ML", "UN" };

    public UnitOfMeasureService(IUnitOfMeasureRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }

    public async Task<UnitOfMeasureListResult> GetAllUnitsAsync()
    {
        try
        {
            var units = await _unitRepository.GetAllAsync();
            var responses = units.Select(MapToResponse).ToList();

            return UnitOfMeasureListResult.Success(responses);
        }
        catch (Exception ex)
        {
            return UnitOfMeasureListResult.Failure(UnitOfMeasureErrors.UnexpectedError("get all units", ex));
        }
    }

    public async Task<UnitOfMeasureResult> GetUnitByIdAsync(Guid id)
    {
        try
        {
            var unit = await _unitRepository.GetByIdAsync(id);
            if (unit == null)
                return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.NotFound(id));

            var response = MapToResponse(unit);
            return UnitOfMeasureResult.Success(response);
        }
        catch (Exception ex)
        {
            return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.UnexpectedError("get unit by id", ex));
        }
    }

    public async Task<UnitOfMeasureResult> CreateUnitAsync(CreateUnitOfMeasureDto request)
    {
        try
        {
            var validationResult = ValidateCreateUnitRequest(request);
            if (validationResult.IsFailed)
                return UnitOfMeasureResult.Failure(validationResult.Errors);

            if (await _unitRepository.ExistsBySymbolAsync(request.Symbol))
                return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.BusinessRules.SymbolAlreadyExists(request.Symbol));

            if (await _unitRepository.ExistsByNameAsync(request.Name))
                return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.BusinessRules.NameAlreadyExists(request.Name));

            var unit = new UnitOfMeasureEntity
            {
                Name = request.Name.Trim(),
                Symbol = request.Symbol.Trim().ToUpperInvariant()
            };

            var createdUnit = await _unitRepository.CreateAsync(unit);
            var response = MapToResponse(createdUnit);

            return UnitOfMeasureResult.Success(response);
        }
        catch (Exception ex)
        {
            return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.UnexpectedError("create unit", ex));
        }
    }

    public async Task<UnitOfMeasureResult> UpdateUnitAsync(Guid id, UpdateUnitOfMeasureDto request)
    {
        try
        {
            var validationResult = ValidateUpdateUnitRequest(request);
            if (validationResult.IsFailed)
                return UnitOfMeasureResult.Failure(validationResult.Errors);

            var unit = await _unitRepository.GetByIdAsync(id);
            if (unit == null)
                return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.NotFound(id));

            // Verificar se é unidade do sistema
            if (SystemUnits.Contains(unit.Symbol.ToUpperInvariant()))
                return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.BusinessRules.SystemUnitCannotBeDeleted(unit.Name));

            if (await _unitRepository.ExistsBySymbolAsync(request.Symbol, id))
                return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.BusinessRules.SymbolAlreadyExists(request.Symbol));

            if (await _unitRepository.ExistsByNameAsync(request.Name, id))
                return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.BusinessRules.NameAlreadyExists(request.Name));

            unit.Name = request.Name.Trim();
            unit.Symbol = request.Symbol.Trim().ToUpperInvariant();

            var updatedUnit = await _unitRepository.UpdateAsync(unit);
            var response = MapToResponse(updatedUnit);

            return UnitOfMeasureResult.Success(response);
        }
        catch (Exception ex)
        {
            return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.UnexpectedError("update unit", ex));
        }
    }

    public async Task<UnitOfMeasureResult> DeleteUnitAsync(Guid id)
    {
        try
        {
            var unit = await _unitRepository.GetByIdAsync(id);
            if (unit == null)
                return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.NotFound(id));

            // Verificar se está sendo usado
            var ingredientCount = await _unitRepository.GetIngredientCountAsync(id);
            if (ingredientCount > 0)
                return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.BusinessRules.CannotDeleteUsedInIngredients(unit.Name, ingredientCount));

            // Verificar se é unidade do sistema
            if (SystemUnits.Contains(unit.Symbol.ToUpperInvariant()))
                return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.BusinessRules.SystemUnitCannotBeDeleted(unit.Name));

            var success = await _unitRepository.DeleteAsync(id);
            if (!success)
                return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.DatabaseError("unit deletion"));

            return UnitOfMeasureResult.Success();
        }
        catch (Exception ex)
        {
            return UnitOfMeasureResult.Failure(UnitOfMeasureErrors.UnexpectedError("delete unit", ex));
        }
    }

    private Result ValidateCreateUnitRequest(CreateUnitOfMeasureDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(UnitOfMeasureErrors.Validation.NameRequired);
        else if (request.Name.Length > 50)
            errors.Add(UnitOfMeasureErrors.Validation.NameTooLong(50));

        if (string.IsNullOrWhiteSpace(request.Symbol))
            errors.Add(UnitOfMeasureErrors.Validation.SymbolRequired);
        else if (request.Symbol.Length > 10)
            errors.Add(UnitOfMeasureErrors.Validation.SymbolTooLong(10));
        else if (!Regex.IsMatch(request.Symbol, @"^[a-zA-Z0-9]+$"))
            errors.Add(UnitOfMeasureErrors.Validation.InvalidSymbolFormat(request.Symbol));

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private Result ValidateUpdateUnitRequest(UpdateUnitOfMeasureDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(UnitOfMeasureErrors.Validation.NameRequired);
        else if (request.Name.Length > 50)
            errors.Add(UnitOfMeasureErrors.Validation.NameTooLong(50));

        if (string.IsNullOrWhiteSpace(request.Symbol))
            errors.Add(UnitOfMeasureErrors.Validation.SymbolRequired);
        else if (request.Symbol.Length > 10)
            errors.Add(UnitOfMeasureErrors.Validation.SymbolTooLong(10));
        else if (!Regex.IsMatch(request.Symbol, @"^[a-zA-Z0-9]+$"))
            errors.Add(UnitOfMeasureErrors.Validation.InvalidSymbolFormat(request.Symbol));

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private UnitOfMeasureResponseDto MapToResponse(UnitOfMeasureEntity unit)
    {
        var ingredientCount = unit.Ingredients?.Count ?? 0;
        var isSystemUnit = SystemUnits.Contains(unit.Symbol.ToUpperInvariant());

        return new UnitOfMeasureResponseDto(
            unit.Id,
            unit.Name,
            unit.Symbol,
            ingredientCount,
            isSystemUnit
        );
    }
}