using MediatR;
using User.Application.Dtos;

namespace User.Application.Commands;

public record RegisterUserCommand(
    string Name,
    string Email,
    string Password
) : IRequest<UserResponseDto?>;

