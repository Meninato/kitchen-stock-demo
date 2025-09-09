using KitchenStock.Application.Abstractions;
using KitchenStock.Application.Kitchen.Queries;
using KitchenStock.Application.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Kitchen.Handlers;

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