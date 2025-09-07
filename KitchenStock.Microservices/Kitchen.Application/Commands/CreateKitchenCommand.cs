using Kitchen.Application.Results;
using MediatR;

namespace Kitchen.Application.Commands;

public record CreateKitchenCommand(
    string Name,
    string? Description,
    bool AllowSharedIngredients = false
) : IRequest<KitchenResult>;
