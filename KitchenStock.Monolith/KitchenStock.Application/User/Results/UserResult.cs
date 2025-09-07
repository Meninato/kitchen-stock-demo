using FluentResults;
using KitchenStock.Application.User.Dtos;

namespace KitchenStock.Application.User.Results;

public class UserResult : Result<UserResponseDto>
{
    public UserResult() : base() { }

    protected UserResult(UserResponseDto value) : base()
    {
        WithValue(value);
    }

    protected UserResult(IError error) : base()
    {
        WithError(error);
    }

    protected UserResult(IEnumerable<IError> errors) : base()
    {
        WithErrors(errors);
    }

    public static UserResult Success(UserResponseDto user) => new(user);
    public static UserResult Failure(IError error) => new(error);
    public static UserResult Failure(IEnumerable<IError> errors) => new(errors);
}
