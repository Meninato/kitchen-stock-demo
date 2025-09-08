using KitchenStock.Application.User.Results;
using MediatR;

namespace KitchenStock.Application.User.Commands;

public record RegisterUserCommand(
    string Name,
    string Email,
    string Password
) : IRequest<UserResult>;
