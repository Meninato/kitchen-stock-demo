using KitchenStock.Application.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Kitchen.Commands;

public record CreateKitchenCommand(Guid UserId, string Name, string Description) 
    : IRequest<KitchenResult>;