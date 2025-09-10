using KitchenStock.Application.Modules.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Queries;

public record GetKitchenByIdQuery(Guid KitchenId, Guid UserId) : IRequest<KitchenResult>;