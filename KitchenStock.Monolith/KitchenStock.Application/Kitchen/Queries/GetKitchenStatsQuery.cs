using KitchenStock.Application.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Kitchen.Queries;

public record GetKitchenStatsQuery(Guid KitchenId, Guid UserId) : IRequest<KitchenStatsResult>;