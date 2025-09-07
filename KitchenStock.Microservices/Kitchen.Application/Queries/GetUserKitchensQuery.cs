using Kitchen.Application.Results;
using MediatR;

namespace Kitchen.Application.Queries;

public record GetUserKitchensQuery(
    Guid UserId,
    bool IncludeInactive = false
) : IRequest<KitchenListResult>;
