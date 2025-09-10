using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Application.Modules.Auth.Dtos;
using KitchenStock.Application.Modules.Auth.MediatR.Commands;
using KitchenStock.Application.Modules.Auth.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Auth.MediatR.Handlers;

public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, AuthResult>
{
    private readonly IAuthService _authService;

    public AuthenticateUserHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResult> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        return await _authService.AuthenticateAsync(MapToDto(request));
    }

    private AuthenticateUserDto MapToDto(AuthenticateUserCommand command)
    {
        return new AuthenticateUserDto(command.Email, command.Password);
    }
}
