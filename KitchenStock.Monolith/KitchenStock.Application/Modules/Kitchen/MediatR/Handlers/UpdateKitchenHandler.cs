using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Kitchen.Dtos;
using KitchenStock.Application.Modules.Kitchen.MediatR.Commands;
using KitchenStock.Application.Modules.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Handlers;

public class UpdateKitchenHandler : IRequestHandler<UpdateKitchenCommand, KitchenResult>
{
    private readonly IKitchenService _kitchenService;

    public UpdateKitchenHandler(IKitchenService kitchenService)
    {
        _kitchenService = kitchenService;
    }

    public async Task<KitchenResult> Handle(UpdateKitchenCommand request, CancellationToken cancellationToken)
    {
        return await _kitchenService.UpdateKitchenAsync(request.KitchenId, request.UserId, MapToDto(request));
    }

    private UpdateKitchenDto MapToDto(UpdateKitchenCommand request)
    {
        return new UpdateKitchenDto(request.Name, request.Description);
    }
}
