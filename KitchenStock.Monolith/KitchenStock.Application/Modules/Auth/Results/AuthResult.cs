using FluentResults;
using KitchenStock.Application.Modules.Auth.Dtos;

namespace KitchenStock.Application.Modules.Auth.Results;

public class AuthResult : Result<AuthResponseDto>
{
    public AuthResult() : base() { }

    protected AuthResult(AuthResponseDto value) : base()
    {
        WithValue(value);
    }

    protected AuthResult(IError error) : base()
    {
        WithError(error);
    }

    protected AuthResult(IEnumerable<IError> errors) : base()
    {
        WithErrors(errors);
    }

    public static AuthResult Success(AuthResponseDto user) => new(user);
    public static AuthResult Success() => new();
    public static AuthResult Failure(IError error) => new(error);
    public static AuthResult Failure(IEnumerable<IError> errors) => new(errors);
}
