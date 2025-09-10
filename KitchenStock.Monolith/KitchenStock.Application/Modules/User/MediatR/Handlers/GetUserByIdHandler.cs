using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Application.Modules.User.MediatR.Queries;
using KitchenStock.Application.Modules.User.Results;
using MediatR;

namespace KitchenStock.Application.Modules.User.MediatR.Handlers;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserResult>
{
    private readonly IUserService _userService;

    public GetUserByIdHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserByIdAsync(request.UserId);
    }
}
