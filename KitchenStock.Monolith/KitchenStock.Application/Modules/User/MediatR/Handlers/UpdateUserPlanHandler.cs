using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Application.Modules.User.Dtos;
using KitchenStock.Application.Modules.User.MediatR.Commands;
using KitchenStock.Application.Modules.User.Results;
using MediatR;

namespace KitchenStock.Application.Modules.User.MediatR.Handlers;

public class UpdateUserPlanHandler : IRequestHandler<UpdateUserPlanCommand, UserResult>
{
    private readonly IUserService _userService;

    public UpdateUserPlanHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserResult> Handle(UpdateUserPlanCommand request, CancellationToken cancellationToken)
    {
        return await _userService.UpdateUserPlanAsync(request.UserId, MapToDto(request));
    }

    private UpdateUserPlanDto MapToDto(UpdateUserPlanCommand request)
    {
        return new UpdateUserPlanDto(request.Plan);
    }
}
