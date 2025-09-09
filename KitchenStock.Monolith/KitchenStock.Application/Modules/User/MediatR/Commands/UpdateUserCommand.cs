using KitchenStock.Application.Modules.User.Results;
using MediatR;

namespace KitchenStock.Application.Modules.User.MediatR.Commands;

public record UpdateUserCommand(
    Guid UserId,
    string Name,
    string Email
) : IRequest<UserResult>;
