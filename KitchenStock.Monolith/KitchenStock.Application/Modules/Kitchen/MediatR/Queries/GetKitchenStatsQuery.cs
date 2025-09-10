using KitchenStock.Application.Modules.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Queries;

public record GetKitchenStatsQuery(Guid KitchenId, Guid UserId) : IRequest<KitchenStatsResult>;