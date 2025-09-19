using FluentResults;
using MediatR;

namespace KitchenStock.Application.Modules.Auth.MediatR.Commands;

public record RevokeAllTokensCommand(Guid UserId, string Reason) : IRequest<Result>;
