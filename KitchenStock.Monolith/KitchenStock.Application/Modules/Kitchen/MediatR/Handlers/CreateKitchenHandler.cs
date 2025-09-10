using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Application.Modules.Kitchen.Dtos;
using KitchenStock.Application.Modules.Kitchen.MediatR.Commands;
using KitchenStock.Application.Modules.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Kitchen.MediatR.Handlers;

public class CreateKitchenHandler : IRequestHandler<CreateKitchenCommand, KitchenResult>
{
    private readonly IKitchenService _kitchenService;

    public CreateKitchenHandler(IKitchenService kitchenService)
    {
        _kitchenService = kitchenService;
    }

    public async Task<KitchenResult> Handle(CreateKitchenCommand request, CancellationToken cancellationToken)
    {
        return await _kitchenService.CreateKitchenAsync(request.UserId, MapToDto(request));
    }

    private CreateKitchenDto MapToDto(CreateKitchenCommand request)
    {
        return new CreateKitchenDto(request.Name, request.Description);
    }
}
