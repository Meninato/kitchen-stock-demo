using KitchenStock.Application.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Kitchen.Commands;

public record UpdateKitchenCommand(Guid KitchenId, Guid UserId, string Name, string Description)
    : IRequest<KitchenResult>;