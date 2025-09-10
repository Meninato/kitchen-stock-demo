using KitchenStock.Application.Modules.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Commands;

public record DeleteKitchenCommand(Guid KitchenId, Guid UserId)
    : IRequest<KitchenResult>;
