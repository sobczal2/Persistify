using System;
using System.Threading.Tasks;

namespace Persistify.Helpers.ErrorHandling;

public readonly struct Result<T>
{
    private readonly T? _value;
    private readonly Exception? _exception;

    public Result(T value)
    {
        _value = value;
        _exception = null;
    }

    public Result(Exception exception)
    {
        _value = default;
        _exception = exception;
    }

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(Exception exception)
    {
        return new Result<T>(exception);
    }

    public static implicit operator T(Result<T> result)
    {
        return result._value ?? throw result._exception!;
    }

    public static implicit operator Exception(Result<T> result)
    {
        return result._exception ?? throw new InvalidOperationException();
    }

    public bool IsSuccess => _exception is null;
    public bool IsFailure => _exception is not null;

    public T Value => _value ?? throw new InvalidOperationException();
    public Exception Exception => _exception ?? throw new InvalidOperationException();

    public Result<T> OnSuccess(Action<T> action)
    {
        if (IsSuccess)
        {
            action(Value);
        }

        return this;
    }

    public Result<T> OnFailure(Action<Exception> action)
    {
        if (IsFailure)
        {
            action(Exception);
        }

        return this;
    }

    public async ValueTask<Result<T>> OnSuccess(Func<T, ValueTask> action)
    {
        if (IsSuccess)
        {
            await action(Value);
        }

        return this;
    }

    public async ValueTask<Result<T>> OnFailure(Func<Exception, ValueTask> action)
    {
        if (IsFailure)
        {
            await action(Exception);
        }

        return this;
    }

    public TRes Match<TRes>(Func<T, TRes> onSuccess, Func<Exception, TRes> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Exception);
    }
}

public readonly struct Result
{
    public static readonly Result Success = new();

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

    public bool IsSuccess => _exception is null;
    public bool IsFailure => _exception is not null;

    public Exception Exception => _exception ?? throw new InvalidOperationException();

    public Result OnSuccess(Action action)
    {
        if (IsSuccess)
        {
            action();
        }

        return this;
    }

    public Result OnFailure(Action<Exception> action)
    {
        if (IsFailure)
        {
            action(Exception);
        }

        return this;
    }

    public async ValueTask<Result> OnSuccess(Func<ValueTask> action)
    {
        if (IsSuccess)
        {
            await action();
        }

        return this;
    }

    public async ValueTask<Result> OnFailure(Func<Exception, ValueTask> action)
    {
        if (IsFailure)
        {
            await action(Exception);
        }

        return this;
    }

    public TRes Match<TRes>(Func<TRes> onSuccess, Func<Exception, TRes> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Exception);
    }
}
