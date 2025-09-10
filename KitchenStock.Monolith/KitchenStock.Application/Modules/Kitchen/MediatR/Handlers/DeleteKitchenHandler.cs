using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Kitchen.MediatR.Commands;
using KitchenStock.Application.Modules.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Handlers;

public class DeleteKitchenHandler : IRequestHandler<DeleteKitchenCommand, KitchenResult>
{
    private readonly IKitchenService _kitchenService;

    public DeleteKitchenHandler(IKitchenService kitchenService)
    {
        _kitchenService = kitchenService;
    }

    public async Task<KitchenResult> Handle(DeleteKitchenCommand request, CancellationToken cancellationToken)
    {
        return await _kitchenService.DeleteKitchenAsync(request.KitchenId, request.UserId);
    }
}
