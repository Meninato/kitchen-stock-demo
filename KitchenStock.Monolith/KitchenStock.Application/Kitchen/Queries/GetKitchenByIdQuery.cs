using KitchenStock.Application.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Kitchen.Queries;

public record GetKitchenByIdQuery(Guid KitchenId, Guid UserId) : IRequest<KitchenResult>;