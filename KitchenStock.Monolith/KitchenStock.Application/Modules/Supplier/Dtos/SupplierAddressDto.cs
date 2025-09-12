namespace KitchenStock.Application.Modules.Supplier.Dtos;

public record SupplierAddressDto(
    string? Street,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    double? Latitude,
    double? Longitude
);