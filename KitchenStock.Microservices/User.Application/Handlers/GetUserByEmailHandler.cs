using MediatR;
using User.Application.Queries;
using User.Application.Results;
using User.Application.Services.Abstractions;

namespace User.Application.Handlers;

public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, UserResult>
{
    private readonly IUserService _userService;

    public GetUserByEmailHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserResult> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetByEmailAsync(request.Email);
    }
}
