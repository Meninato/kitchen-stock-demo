using MediatR;
using User.Application.Results;

namespace User.Application.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserResult>;