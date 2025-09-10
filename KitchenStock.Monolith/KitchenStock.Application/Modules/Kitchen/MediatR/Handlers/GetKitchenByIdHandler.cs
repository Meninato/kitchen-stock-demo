using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Kitchen.MediatR.Queries;
using KitchenStock.Application.Modules.Kitchen.Results;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Handlers;

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
