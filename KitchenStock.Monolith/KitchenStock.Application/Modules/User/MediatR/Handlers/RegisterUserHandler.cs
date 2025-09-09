using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Application.Modules.User.Dtos;
using KitchenStock.Application.Modules.User.MediatR.Commands;
using KitchenStock.Application.Modules.User.Results;
using MediatR;

namespace KitchenStock.Application.Modules.User.MediatR.Handlers;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserResult>
{
    private readonly IUserService _userService;

    public RegisterUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.CreateUserAsync(MapToDto(request));
    }

    private CreateUserDto MapToDto(RegisterUserCommand request)
    {
        return new CreateUserDto(request.Name, request.Email, request.Password);
    }
}