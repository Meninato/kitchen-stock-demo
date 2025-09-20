using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Application.Modules.User.Dtos;
using KitchenStock.Application.Modules.User.MediatR.Commands;
using KitchenStock.Application.Modules.User.Results;
using MediatR;

namespace KitchenStock.Application.Modules.User.MediatR.Handlers;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserResult>
{
    private readonly IUserService _userService;

    public CreateUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.CreateUserAsync(MapToDto(request));
    }

    private CreateUserDto MapToDto(CreateUserCommand request)
    {
        return new CreateUserDto(request.Name, request.Email, request.Password);
    }
}