using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Kitchen.MediatR.Queries;
using KitchenStock.Application.Modules.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Handlers;

public class GetUserKitchensHandler : IRequestHandler<GetUserKitchensQuery, KitchenListResult>
{
    private readonly IKitchenService _kitchenService;

    public GetUserKitchensHandler(IKitchenService kitchenService)
    {
        _kitchenService = kitchenService;
    }

    public async Task<KitchenListResult> Handle(GetUserKitchensQuery request, CancellationToken cancellationToken)
    {
        return await _kitchenService.GetUserKitchensAsync(request.UserId);
    }
}
