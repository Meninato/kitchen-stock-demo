namespace KitchenStock.Application.Modules.Supplier.Dtos;

public record CreateSupplierDto(
    Guid KitchenId,
    string Name,
    SupplierContactDto? Contact,
    SupplierAddressDto? Address
);