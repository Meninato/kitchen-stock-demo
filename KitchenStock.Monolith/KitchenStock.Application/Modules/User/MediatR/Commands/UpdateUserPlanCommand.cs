using KitchenStock.Application.Modules.User.Results;
using KitchenStock.Domain.Enums;
using MediatR;

namespace KitchenStock.Application.Modules.User.MediatR.Commands;

public record UpdateUserPlanCommand(Guid UserId, UserPlan Plan) : IRequest<UserResult>;
