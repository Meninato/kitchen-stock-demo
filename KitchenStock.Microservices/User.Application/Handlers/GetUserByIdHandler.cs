using MediatR;
using User.Application.Queries;
using User.Application.Results;
using User.Application.Services.Abstractions;

namespace User.Application.Handlers;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserResult>
{
    private readonly IUserService _userService;

    public GetUserByIdHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetByIdAsync(request.UserId);
    }
}
