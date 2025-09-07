using KitchenStock.Application.Abstractions;
using KitchenStock.Application.Auth.Commands;
using KitchenStock.Application.Auth.Results;
using MediatR;

namespace KitchenStock.Application.Auth.Handlers;

public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, AuthResult>
{
    private readonly IAuthService _authService;

    public AuthenticateUserHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResult> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        return await _authService.AuthenticateAsync(request.Email, request.Password);
    }
}
