using KitchenStock.Application.Abstractions;
using KitchenStock.Application.Kitchen.Queries;
using KitchenStock.Application.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Kitchen.Handlers;

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
