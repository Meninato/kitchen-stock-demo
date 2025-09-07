using KitchenStock.Application.Auth.Results;
using MediatR;

namespace KitchenStock.Application.Auth.Commands;

public record AuthenticateUserCommand(
    string Email,
    string Password
) : IRequest<AuthResult>;

