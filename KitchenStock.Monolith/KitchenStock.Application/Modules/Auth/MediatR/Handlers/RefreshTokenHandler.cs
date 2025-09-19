using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Application.Modules.Auth.MediatR.Commands;
using KitchenStock.Application.Modules.Auth.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Auth.MediatR.Handlers;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private readonly IAuthService _authService;

    public RefreshTokenHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RefreshTokenAsync(request.TokenValue, request.IpAddress, request.UserAgent);
    }
}
