using FluentResults;
using MediatR;

namespace KitchenStock.Application.Modules.Auth.MediatR.Commands;

public record LogoutCommand(string TokenValue) : IRequest<Result>;
