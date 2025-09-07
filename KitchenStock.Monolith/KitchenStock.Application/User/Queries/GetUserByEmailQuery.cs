using KitchenStock.Application.User.Results;
using MediatR;

namespace KitchenStock.Application.User.Queries;

public record GetUserByEmailQuery(string Email) : IRequest<UserResult>;