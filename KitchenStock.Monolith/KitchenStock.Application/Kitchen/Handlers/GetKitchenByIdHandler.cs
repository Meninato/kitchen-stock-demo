using KitchenStock.Application.Abstractions;
using KitchenStock.Application.Kitchen.Queries;
using KitchenStock.Application.Kitchen.Results;

namespace KitchenStock.Application.Kitchen.Handlers;

internal class GetKitchenByIdHandler
{
    private readonly IKitchenService _kitchenService;

    public GetKitchenByIdHandler(IKitchenService kitchenService)
    {
        _kitchenService = kitchenService;
    }

    public async Task<KitchenResult> Handle(GetKitchenByIdQuery request, CancellationToken cancellationToken)
    {
        return await _kitchenService.GetKitchenByIdAsync(request.KitchenId, request.UserId);
    }
}
