using MediatR;
using User.Application.Commands;
using User.Application.Dtos;
using User.Application.Results;
using User.Application.Services.Abstractions;

namespace User.Application.Handlers;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, AuthResult>
{
    private readonly IUserService _userService;

    public LoginUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<AuthResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.LoginAsync(MapToDto(request));
    }

    private LoginUserDto MapToDto(LoginUserCommand request)
    {
        return new LoginUserDto(request.Email, request.Password);
    }
}
