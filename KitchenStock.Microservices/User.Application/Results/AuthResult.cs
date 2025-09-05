using FluentResults;

namespace User.Application.Results;

public class AuthResult : Result<string>
{
    public AuthResult() : base() { }

    protected AuthResult(string token) : base()
    {
        WithValue(token);
    }

    protected AuthResult(IError error) : base()
    {
        WithError(error);
    }

    protected AuthResult(IEnumerable<IError> errors) : base()
    {
        WithErrors(errors);
    }

    public static AuthResult Success(string token) => new(token);
    public static AuthResult Failure(IError error) => new(error);
    public static AuthResult Failure(IEnumerable<IError> errors) => new(errors);
}
