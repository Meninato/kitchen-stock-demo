using KitchenStock.Application.Modules.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Commands;

public record CreateKitchenCommand(Guid UserId, string Name, string? Description) 
    : IRequest<KitchenResult>;