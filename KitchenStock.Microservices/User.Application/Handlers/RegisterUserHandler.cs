using MediatR;
using User.Application.Commands;
using User.Application.Dtos;
using User.Application.Services.Abstractions;

namespace User.Application.Handlers;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserResponseDto?>
{
    private readonly IUserService _userService;

    public RegisterUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserResponseDto?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
