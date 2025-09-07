using KitchenStock.Domain.Enums;

namespace KitchenStock.Application.User.Dtos;

public record RegisterUserDto(
    string Name, 
    string Email, 
    string Password, 
    UserPlan UserPlan = UserPlan.Basic);
