using FluentResults;
using KitchenStock.Application.Modules.Auth.Abstractions;
using KitchenStock.Application.Modules.Auth.MediatR.Commands;
using MediatR;

namespace KitchenStock.Application.Modules.Auth.MediatR.Handlers;

public class RevokeAllTokensHandler : IRequestHandler<RevokeAllTokensCommand, Result>
{
    private readonly IAuthService _authService;

    public RevokeAllTokensHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result> Handle(RevokeAllTokensCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RevokeAllUserTokensAsync(request.UserId, request.Reason);
    }
}