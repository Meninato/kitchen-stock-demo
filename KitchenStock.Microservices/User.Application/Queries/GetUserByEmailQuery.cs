using MediatR;
using User.Application.Results;

namespace User.Application.Queries;

public record GetUserByEmailQuery(string Email) : IRequest<UserResult>;