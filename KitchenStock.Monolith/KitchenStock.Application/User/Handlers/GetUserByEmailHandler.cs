using KitchenStock.Application.Abstractions;
using KitchenStock.Application.User.Queries;
using KitchenStock.Application.User.Results;
using MediatR;

namespace KitchenStock.Application.User.Handlers;

public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, KitchenResult>
{
    private readonly IUserService _userService;

    public GetUserByEmailHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<KitchenResult> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetByEmailAsync(request.Email);
    }
}
