using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Application.Modules.User.Dtos;
using KitchenStock.Application.Modules.User.MediatR.Commands;
using KitchenStock.Application.Modules.User.Results;
using MediatR;

namespace KitchenStock.Application.Modules.User.MediatR.Handlers;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserResult>
{
    private readonly IUserService _userService;

    public UpdateUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.UpdateUserAsync(request.UserId, MapToDto(request));
    }

    private UpdateUserDto MapToDto(UpdateUserCommand request)
    {
        return new UpdateUserDto(request.Name, request.Email);
    }
}
