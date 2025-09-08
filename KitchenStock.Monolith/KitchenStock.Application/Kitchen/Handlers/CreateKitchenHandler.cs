using KitchenStock.Application.Abstractions;
using KitchenStock.Application.Kitchen.Commands;
using KitchenStock.Application.Kitchen.Dtos;
using KitchenStock.Application.Kitchen.Results;
using MediatR;

namespace KitchenStock.Application.Kitchen.Handlers;

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
