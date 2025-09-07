using KitchenStock.Application.User.Results;
using MediatR;

namespace KitchenStock.Application.User.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<KitchenResult>;