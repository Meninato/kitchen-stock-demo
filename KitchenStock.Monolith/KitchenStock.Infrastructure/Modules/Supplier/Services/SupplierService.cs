using FluentResults;
using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Supplier.Abstractions;
using KitchenStock.Application.Modules.Supplier.Dtos;
using KitchenStock.Application.Modules.Supplier.Results;
using KitchenStock.Domain.Entities;
using KitchenStock.Domain.Enums;
using KitchenStock.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace KitchenStock.Infrastructure.Modules.Supplier.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IKitchenRepository _kitchenRepository;

    public SupplierService(
        ISupplierRepository supplierRepository,
        IKitchenRepository kitchenRepository)
    {
        _supplierRepository = supplierRepository;
        _kitchenRepository = kitchenRepository;
    }

    public async Task<SupplierResult> CreateSupplierAsync(Guid userId, CreateSupplierDto request)
    {
        try
        {
            var validationResult = ValidateCreateSupplierRequest(request);
            if (validationResult.IsFailed)
                return SupplierResult.Failure(validationResult.Errors);

            if (!await _kitchenRepository.UserOwnsKitchenAsync(request.KitchenId, userId))
                return SupplierResult.Failure(SupplierErrors.Authorization.KitchenAccessDenied(request.KitchenId, userId));

            if (await _supplierRepository.ExistsInKitchenAsync(request.Name, request.KitchenId))
                return SupplierResult.Failure(SupplierErrors.BusinessRules.AlreadyExistsInKitchen(request.Name, request.KitchenId));

            if (!string.IsNullOrWhiteSpace(request.Email) &&
                await _supplierRepository.EmailExistsInKitchenAsync(request.Email, request.KitchenId))
                return SupplierResult.Failure(SupplierErrors.BusinessRules.EmailAlreadyUsedInKitchen(request.Email, request.KitchenId));

            if (!string.IsNullOrWhiteSpace(request.Phone) &&
                await _supplierRepository.PhoneExistsInKitchenAsync(request.Phone, request.KitchenId))
                return SupplierResult.Failure(SupplierErrors.BusinessRules.PhoneAlreadyUsedInKitchen(request.Phone, request.KitchenId));

            var supplier = new SupplierEntity
            {
                Name = request.Name.Trim(),
                SupplierContact = new Contact(
                    request.Email?.Trim() ?? string.Empty,
                    request.Phone?.Trim() ?? string.Empty
                ),
                SupplierAddress = new Address(
                    
                ),
                KitchenId = request.KitchenId
            };

            var createdSupplier = await _supplierRepository.CreateAsync(supplier);
            var supplierWithDetails = await _supplierRepository.GetByIdAsync(createdSupplier.Id);
            var response = MapToResponse(supplierWithDetails!);

            return SupplierResult.Success(response);
        }
        catch (Exception ex)
        {
            return SupplierResult.Failure(SupplierErrors.UnexpectedError("supplier creation", ex));
        }
    }

    public async Task<SupplierResult> GetSupplierByIdAsync(int id, int userId)
    {
        try
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return SupplierResult.Failure(SupplierErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(supplier.KitchenId, userId))
                return SupplierResult.Failure(SupplierErrors.Authorization.KitchenAccessDenied(supplier.KitchenId, userId));

            var response = MapToResponse(supplier);
            return SupplierResult.Success(response);
        }
        catch (Exception ex)
        {
            return SupplierResult.Failure(SupplierErrors.UnexpectedError("get supplier by id", ex));
        }
    }

    public async Task<SupplierListResult> GetKitchenSuppliersAsync(int kitchenId, int userId)
    {
        try
        {
            if (!await _kitchenRepository.UserOwnsKitchenAsync(kitchenId, userId))
                return SupplierListResult.Failure(SupplierErrors.Authorization.KitchenAccessDenied(kitchenId, userId));

            var suppliers = await _supplierRepository.GetByKitchenIdAsync(kitchenId);
            var responses = suppliers.Select(MapToResponse).ToList();

            return SupplierListResult.Success(responses);
        }
        catch (Exception ex)
        {
            return SupplierListResult.Failure(SupplierErrors.UnexpectedError("get kitchen suppliers", ex));
        }
    }

    public async Task<SupplierResult> UpdateSupplierAsync(int id, int userId, UpdateSupplierRequest request)
    {
        try
        {
            var validationResult = ValidateUpdateSupplierRequest(request);
            if (validationResult.IsFailed)
                return SupplierResult.Failure(validationResult.Errors);

            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return SupplierResult.Failure(SupplierErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(supplier.KitchenId, userId))
                return SupplierResult.Failure(SupplierErrors.Authorization.KitchenAccessDenied(supplier.KitchenId, userId));

            if (await _supplierRepository.ExistsInKitchenAsync(request.Name, supplier.KitchenId, id))
                return SupplierResult.Failure(SupplierErrors.BusinessRules.AlreadyExistsInKitchen(request.Name, supplier.KitchenId));

            if (!string.IsNullOrWhiteSpace(request.Email) &&
                await _supplierRepository.EmailExistsInKitchenAsync(request.Email, supplier.KitchenId, id))
                return SupplierResult.Failure(SupplierErrors.BusinessRules.EmailAlreadyUsedInKitchen(request.Email, supplier.KitchenId));

            if (!string.IsNullOrWhiteSpace(request.Phone) &&
                await _supplierRepository.PhoneExistsInKitchenAsync(request.Phone, supplier.KitchenId, id))
                return SupplierResult.Failure(SupplierErrors.BusinessRules.PhoneAlreadyUsedInKitchen(request.Phone, supplier.KitchenId));

            supplier.Name = request.Name.Trim();
            supplier.Phone = request.Phone?.Trim() ?? string.Empty;
            supplier.Email = request.Email?.Trim() ?? string.Empty;
            supplier.Address = request.Address?.Trim() ?? string.Empty;

            var updatedSupplier = await _supplierRepository.UpdateAsync(supplier);
            var response = MapToResponse(updatedSupplier);

            return SupplierResult.Success(response);
        }
        catch (Exception ex)
        {
            return SupplierResult.Failure(SupplierErrors.UnexpectedError("supplier update", ex));
        }
    }

    public async Task<SupplierResult> DeleteSupplierAsync(int id, int userId)
    {
        try
        {
            var supplier = await _supplierRepository.GetByIdWithStockEntriesAsync(id);
            if (supplier == null)
                return SupplierResult.Failure(SupplierErrors.Authorization.NotFound(id));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(supplier.KitchenId, userId))
                return SupplierResult.Failure(SupplierErrors.Authorization.KitchenAccessDenied(supplier.KitchenId, userId));

            if (supplier.StockEntries?.Any() == true)
                return SupplierResult.Failure(SupplierErrors.BusinessRules.CannotDeleteWithActiveStockEntries(supplier.Name, supplier.StockEntries.Count));

            var success = await _supplierRepository.DeleteAsync(id);
            if (!success)
                return SupplierResult.Failure(SupplierErrors.DatabaseError("supplier deletion"));

            var emptyResponse = new SupplierResponse(id, "", "", "", "", 0, 0, null, DateTime.MinValue);
            return SupplierResult.Success(emptyResponse);
        }
        catch (Exception ex)
        {
            return SupplierResult.Failure(SupplierErrors.UnexpectedError("supplier deletion", ex));
        }
    }

    public async Task<SupplierPerformanceResult> GetSupplierPerformanceAsync(int supplierId, int userId, DateTime? fromDate = null)
    {
        try
        {
            var supplier = await _supplierRepository.GetByIdWithStockEntriesAsync(supplierId);
            if (supplier == null)
                return SupplierPerformanceResult.Failure(SupplierErrors.Authorization.NotFound(supplierId));

            if (!await _kitchenRepository.UserOwnsKitchenAsync(supplier.KitchenId, userId))
                return SupplierPerformanceResult.Failure(SupplierErrors.Authorization.KitchenAccessDenied(supplier.KitchenId, userId));

            var stockEntries = supplier.StockEntries?
                .Where(se => se.MovementType == StockMovementType.Purchase && se.UnitPrice.HasValue)
                .Where(se => !fromDate.HasValue || se.MovementDate >= fromDate.Value)
                .ToList() ?? new List<StockEntry>();

            var totalPurchases = stockEntries.Count;
            var totalValue = stockEntries.Sum(se => se.Quantity * se.UnitPrice!.Value);
            var averageOrderValue = totalPurchases > 0 ? totalValue / totalPurchases : 0;
            var firstPurchase = stockEntries.OrderBy(se => se.MovementDate).FirstOrDefault()?.MovementDate;
            var lastPurchase = stockEntries.OrderByDescending(se => se.MovementDate).FirstOrDefault()?.MovementDate;

            // Monthly breakdown
            var monthlyBreakdown = stockEntries
                .GroupBy(se => se.MovementDate.ToString("yyyy-MM"))
                .Select(g => new MonthlyPurchaseSummary(
                    g.Key,
                    g.Count(),
                    g.Sum(se => se.Quantity * se.UnitPrice!.Value)
                ))
                .OrderBy(m => m.Month)
                .ToList();

            // Top ingredients
            var topIngredients = stockEntries
                .GroupBy(se => se.Ingredient)
                .Select(g => new IngredientPurchaseSummary(
                    g.Key.Name,
                    g.Sum(se => se.Quantity),
                    g.Key.UnitOfMeasure?.Symbol ?? "UN",
                    g.Sum(se => se.Quantity * se.UnitPrice!.Value),
                    g.Average(se => se.UnitPrice!.Value)
                ))
                .OrderByDescending(i => i.TotalValue)
                .Take(10)
                .ToList();

            var performance = new SupplierPerformanceResponse(
                supplier.Id,
                supplier.Name,
                totalPurchases,
                totalValue,
                averageOrderValue,
                firstPurchase,
                lastPurchase,
                monthlyBreakdown,
                topIngredients
            );

            return SupplierPerformanceResult.Success(performance);
        }
        catch (Exception ex)
        {
            return SupplierPerformanceResult.Failure(SupplierErrors.UnexpectedError("supplier performance analysis", ex));
        }
    }

    public async Task<SupplierListResult> SearchSuppliersAsync(string searchTerm, int kitchenId, int userId)
    {
        try
        {
            if (!await _kitchenRepository.UserOwnsKitchenAsync(kitchenId, userId))
                return SupplierListResult.Failure(SupplierErrors.Authorization.KitchenAccessDenied(kitchenId, userId));

            var suppliers = await _supplierRepository.SearchByNameAsync(searchTerm, kitchenId);
            var responses = suppliers.Select(MapToResponse).ToList();

            return SupplierListResult.Success(responses);
        }
        catch (Exception ex)
        {
            return SupplierListResult.Failure(SupplierErrors.UnexpectedError("supplier search", ex));
        }
    }

    private Result ValidateCreateSupplierRequest(CreateSupplierDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(SupplierErrors.Validation.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(SupplierErrors.Validation.NameTooLong(100));

        if(request.Contact is not null)
        {
            if (!string.IsNullOrEmpty(request.Contact.Phone))
            {
                if (request.Contact.Phone.Length > 50)
                    errors.Add(SupplierErrors.Validation.PhoneTooLong(50));
                else if (!IsValidPhone(request.Contact.Phone))
                    errors.Add(SupplierErrors.Validation.InvalidPhoneFormat(request.Contact.Phone));
            }

            if (!string.IsNullOrEmpty(request.Contact.Email))
            {
                if (request.Contact.Email.Length > 254)
                    errors.Add(SupplierErrors.Validation.EmailTooLong(254));
                else if (!IsValidEmail(request.Contact.Email))
                    errors.Add(SupplierErrors.Validation.InvalidEmailFormat(request.Contact.Email));
            }
        }

        if(request.Address is not null)
        {
            if (!string.IsNullOrEmpty(request.Address) && request.Address.Length > 200)
                errors.Add(SupplierErrors.Validation.AddressTooLong(200));
        }

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private Result ValidateUpdateSupplierRequest(UpdateSupplierDto request)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(SupplierErrors.Validation.NameRequired);
        else if (request.Name.Length > 100)
            errors.Add(SupplierErrors.Validation.NameTooLong(100));

        if (!string.IsNullOrEmpty(request.Phone))
        {
            if (request.Phone.Length > 20)
                errors.Add(SupplierErrors.Validation.PhoneTooLong(20));
            else if (!IsValidPhone(request.Phone))
                errors.Add(SupplierErrors.Validation.InvalidPhoneFormat(request.Phone));
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            if (request.Email.Length > 100)
                errors.Add(SupplierErrors.Validation.EmailTooLong(100));
            else if (!IsValidEmail(request.Email))
                errors.Add(SupplierErrors.Validation.InvalidEmailFormat(request.Email));
        }

        if (!string.IsNullOrEmpty(request.Address) && request.Address.Length > 200)
            errors.Add(SupplierErrors.Validation.AddressTooLong(200));

        return errors.Any() ? Result.Fail(errors) : Result.Ok();
    }

    private bool IsValidEmail(string email)
    {
        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailRegex);
    }

    private bool IsValidPhone(string phone)
    {
        // Simple phone validation - allows numbers, spaces, dashes, parentheses, and dots
        var phoneRegex = @"^[\d\s\-\(\)\.+]+$";
        return Regex.IsMatch(phone, phoneRegex) && phone.Any(char.IsDigit);
    }

    private SupplierResponseDto MapToResponse(SupplierEntity supplier)
    {
        var stockEntries = supplier.StockEntries?.Where(se => se.MovementType == StockMovementType.Purchase && se.UnitPrice.HasValue) ?? new List<StockEntryEntity>();
        var totalPurchaseValue = stockEntries.Sum(se => se.Quantity * se.UnitPrice!.Value);
        var lastPurchaseDate = stockEntries.OrderByDescending(se => se.MovementDate).FirstOrDefault()?.MovementDate;

        return new SupplierResponseDto(
            supplier.Id,
            supplier.Name,
            supplier.SupplierContact,
            supplier.SupplierAddress,
            stockEntries.Count(),
            totalPurchaseValue,
            lastPurchaseDate,
            supplier.CreatedAt
        );
    }
}