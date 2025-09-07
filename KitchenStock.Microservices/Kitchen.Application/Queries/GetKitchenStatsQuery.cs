using Kitchen.Application.Results;
using MediatR;

namespace Kitchen.Application.Queries;

public record GetKitchenStatsQuery(
    Guid KitchenId
) : IRequest<KitchenStatsResult>;
