using MediatR;
using KitchenStock.Application.Abstractions;
using KitchenStock.Application.User.Dtos;
using KitchenStock.Application.User.Results;
using KitchenStock.Application.User.Commands;

namespace KitchenStock.Application.User.Handlers;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserResult>
{
    private readonly IUserService _userService;

    public RegisterUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.RegisterAsync(MapToDto(request));
    }

    private RegisterUserDto MapToDto(RegisterUserCommand request)
    {
        return new RegisterUserDto(request.Name, request.Email, request.Password);
    }
}
