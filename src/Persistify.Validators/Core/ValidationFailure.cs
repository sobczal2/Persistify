namespace Persistify.Validators.Core;

public struct ValidationFailure
{
    public string ErrorCode { get; }

    public ValidationFailure(string errorCode)
    {
        ErrorCode = errorCode;
    }
}