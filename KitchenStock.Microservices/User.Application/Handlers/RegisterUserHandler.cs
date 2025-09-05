using MediatR;
using User.Application.Commands;
using User.Application.Dtos;
using User.Application.Results;
using User.Application.Services.Abstractions;

namespace User.Application.Handlers;

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
