using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Kitchen.MediatR.Queries;
using KitchenStock.Application.Modules.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Handlers;

public class GetKitchenByIdHandler : IRequestHandler<GetKitchenByIdQuery, KitchenResult>
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
