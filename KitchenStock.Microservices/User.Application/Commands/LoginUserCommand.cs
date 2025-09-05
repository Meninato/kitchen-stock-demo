using MediatR;
using User.Application.Results;

namespace User.Application.Commands;

public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<AuthResult>;
