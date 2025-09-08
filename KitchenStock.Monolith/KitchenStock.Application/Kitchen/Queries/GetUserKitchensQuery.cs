
using KitchenStock.Application.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Kitchen.Queries;

public record GetUserKitchensQuery(Guid UserId) : IRequest<KitchenListResult>;
