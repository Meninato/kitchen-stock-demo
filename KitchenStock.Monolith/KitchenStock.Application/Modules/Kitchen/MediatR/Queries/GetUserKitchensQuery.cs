using KitchenStock.Application.Modules.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Queries;

public record GetUserKitchensQuery(Guid UserId) : IRequest<KitchenListResult>;
