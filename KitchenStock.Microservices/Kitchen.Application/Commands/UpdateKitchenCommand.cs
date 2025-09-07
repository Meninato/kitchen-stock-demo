using Kitchen.Application.Results;
using MediatR;

namespace Kitchen.Application.Commands;

public record UpdateKitchenCommand(
    Guid KitchenId,
    string Name,
    string? Description,
    bool AllowSharedIngredients = false
) : IRequest<KitchenResult>;
