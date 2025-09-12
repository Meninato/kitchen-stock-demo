namespace KitchenStock.Application.Modules.Supplier.Dtos;

public record UpdateSupplierDto(
    string Name,
    string Phone,
    string Email,
    string Address
);