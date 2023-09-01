using System;
using System.Runtime.ExceptionServices;

namespace Persistify.Server.Validation.Results;

public readonly struct Result
{
    public static Result Ok => new();

    private readonly Exception? _exception;

    public Result(Exception exception)
    {
        _exception = exception;
    }

    public static implicit operator Result(Exception exception)
    {
        return new Result(exception);
    }

    public static implicit operator Exception(Result result)
    {
        return result._exception ?? throw new InvalidOperationException();
    }

    public bool Success => _exception is null;
    public bool Failure => !Success;

    public Result OnSuccess(Action action)
    {
        if (Success)
        {
            action();
        }

        return this;
    }

    public Result OnFailure(Action<Exception> action)
    {
        var exception = _exception ?? throw new InvalidOperationException();
        if (Failure)
        {
            action(exception);
        }

        return this;
    }

    public TRes Match<TRes>(Func<TRes> onSuccess, Func<Exception, TRes> onFailure)
    {
        return Success ? onSuccess() : onFailure(_exception!);
    }

    public void Throw()
    {
        var exception = _exception ?? throw new InvalidOperationException();
        throw exception;
    }

    public void ThrowSuppressed()
    {
        var exception = _exception ?? throw new InvalidOperationException();
        var edi = ExceptionDispatchInfo.Capture(exception);
        edi.Throw();
    }
}
