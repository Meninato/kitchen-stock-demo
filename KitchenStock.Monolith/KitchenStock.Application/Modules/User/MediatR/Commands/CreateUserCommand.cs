using KitchenStock.Application.Modules.User.Results;
using KitchenStock.Domain.Enums;
using MediatR;

namespace KitchenStock.Application.Modules.User.MediatR.Commands;

public record CreateUserCommand(
    string Name,
    string Email,
    string Password,
    UserPlan Plan = UserPlan.Basic
) : IRequest<UserResult>;
