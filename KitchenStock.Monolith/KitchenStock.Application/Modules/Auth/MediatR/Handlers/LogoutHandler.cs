using FluentResults;
using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Application.Modules.Auth.MediatR.Commands;
using KitchenStock.Application.Modules.Auth.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Auth.MediatR.Handlers;

public class LogoutHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IAuthService _authService;

    public LogoutHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LogoutAsync(request.TokenValue);
    }
}
