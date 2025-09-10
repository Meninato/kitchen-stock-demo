using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Kitchen.MediatR.Queries;
using KitchenStock.Application.Modules.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Handlers;

internal class GetKitchenStatsHandler : IRequestHandler<GetKitchenStatsQuery, KitchenStatsResult>
{
    private readonly IKitchenService _kitchenService;

    public GetKitchenStatsHandler(IKitchenService kitchenService)
    {
        _kitchenService = kitchenService;
    }

    public async Task<KitchenStatsResult> Handle(GetKitchenStatsQuery request, CancellationToken cancellationToken)
    {
        return await _kitchenService.GetKitchenStatsAsync(request.KitchenId, request.UserId);
    }
}