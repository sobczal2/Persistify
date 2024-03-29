﻿using System;
using System.Threading.Tasks;

namespace Persistify.Helpers.Results;

public readonly struct Result<T>
{
    private readonly T? _value;
    private readonly Exception? _exception;

    public T Value => _value ?? throw new InvalidOperationException();
    public Exception Exception => _exception ?? throw new InvalidOperationException();

    public Result(
        T value
    )
    {
        _value = value;
        _exception = null;
    }

    public Result(
        Exception exception
    )
    {
        _value = default;
        _exception = exception;
    }

    public static implicit operator Result<T>(
        T value
    )
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(
        Exception exception
    )
    {
        return new Result<T>(exception);
    }

    public static implicit operator T(
        Result<T> result
    )
    {
        return result._value ?? throw new InvalidOperationException();
    }

    public static implicit operator Exception(
        Result<T> result
    )
    {
        return result._exception ?? throw new InvalidOperationException();
    }

    public bool Success => _exception is null;
    public bool Failure => !Success;

    public Result<T> OnSuccess(
        Action<T> action
    )
    {
        if (Success)
        {
            var value = _value ?? throw new InvalidOperationException();
            action(value);
        }

        return this;
    }

    public Result<T> OnFailure(
        Action<Exception> action
    )
    {
        if (Failure)
        {
            var exception = _exception ?? throw new InvalidOperationException();
            action(exception);
        }

        return this;
    }

    public TRes Match<TRes>(
        Func<T, TRes> onSuccess,
        Func<Exception, TRes> onFailure
    )
    {
        return Success ? onSuccess(_value!) : onFailure(_exception!);
    }

    public static Result<T> From(
        Func<T> func
    )
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static async Task<Result<T>> FromAsync(
        Func<Task<T>> func
    )
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
