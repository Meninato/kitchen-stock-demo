namespace KitchenStock.Application.Kitchen.Dtos;

public record UpdateKitchenDto(Guid KitchenId, Guid UserId, string Name, string Description);
