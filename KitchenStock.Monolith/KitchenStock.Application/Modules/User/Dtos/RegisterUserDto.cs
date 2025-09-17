namespace KitchenStock.Application.Modules.User.Dtos;

public record RegisterUserDto(
    string Name,
    string Email,
    string Password
);