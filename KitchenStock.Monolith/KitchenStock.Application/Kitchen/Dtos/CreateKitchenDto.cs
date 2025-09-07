namespace KitchenStock.Application.Kitchen.Dtos;

public record CreateKitchenDto(Guid UserId, string Name, string Description);