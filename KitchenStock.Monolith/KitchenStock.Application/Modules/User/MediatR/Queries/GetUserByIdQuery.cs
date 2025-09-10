using KitchenStock.Application.Modules.User.Results;
using MediatR;

namespace KitchenStock.Application.Modules.User.MediatR.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserResult>;
