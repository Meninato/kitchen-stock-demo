using KitchenStock.Application.Modules.Auth.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Auth.MediatR.Commands;

public record RefreshTokenCommand(string TokenValue, string IpAddress, string UserAgent) : IRequest<AuthResult>;