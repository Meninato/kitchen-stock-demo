using KitchenStock.Application.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Kitchen.Commands;

public record DeleteKitchenCommand(Guid KitchenId, Guid UserId)
    : IRequest<KitchenResult>;
