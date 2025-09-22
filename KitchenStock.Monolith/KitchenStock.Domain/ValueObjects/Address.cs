
namespace KitchenStock.Domain.ValueObjects;

public record Address(
    string? Street = null,
    string? City = null,
    string? State = null,
    string? PostalCode = null,
    string? Country = null,
    double? Latitude = null,
    double? Longitude = null
);