using KitchenStock.Application.Abstractions;
using KitchenStock.Application.User.Queries;
using KitchenStock.Application.User.Results;
using MediatR;

namespace KitchenStock.Application.User.Handlers;

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
