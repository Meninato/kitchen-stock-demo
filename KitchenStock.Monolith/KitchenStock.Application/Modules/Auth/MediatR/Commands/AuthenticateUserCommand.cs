using KitchenStock.Application.Modules.Auth.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Auth.MediatR.Commands;

public record AuthenticateUserCommand(
    string Email,
    string Password
) : IRequest<AuthResult>;

