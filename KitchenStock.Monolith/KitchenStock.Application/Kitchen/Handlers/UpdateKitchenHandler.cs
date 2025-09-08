using KitchenStock.Application.Abstractions;
using KitchenStock.Application.Kitchen.Commands;
using KitchenStock.Application.Kitchen.Dtos;
using KitchenStock.Application.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Kitchen.Handlers;

public class UpdateKitchenHandler : IRequestHandler<UpdateKitchenCommand, KitchenResult>
{
    private readonly IKitchenService _kitchenService;

    public UpdateKitchenHandler(IKitchenService kitchenService)
    {
        _kitchenService = kitchenService;
    }

    public async Task<KitchenResult> Handle(UpdateKitchenCommand request, CancellationToken cancellationToken)
    {
        return await _kitchenService.UpdateKitchenAsync(request.UserId, MapToDto(request));
    }

    private UpdateKitchenDto MapToDto(UpdateKitchenCommand request)
    {
        return new UpdateKitchenDto(request.KitchenId, request.Name, request.Description);
    }
}
