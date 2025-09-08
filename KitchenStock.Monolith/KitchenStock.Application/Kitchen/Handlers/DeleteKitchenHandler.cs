using KitchenStock.Application.Abstractions;
using KitchenStock.Application.Kitchen.Commands;
using KitchenStock.Application.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Kitchen.Handlers;

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
