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
    
    public static implicit operator Result<T>(T value) => new(value);
    public static implicit operator Result<T>(Exception exception) => new(exception);
    
    public static implicit operator T(Result<T> result) => result._value ?? throw result._exception!;
    public static implicit operator Exception(Result<T> result) => result._exception ?? throw new InvalidOperationException();
    
    public bool IsSuccess => _value is not null;
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
    
    public T Match(Func<T, T> onSuccess, Func<Exception, T> onFailure)
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
    
    public static implicit operator Result(Exception exception) => new(exception);
    
    public static implicit operator Exception(Result result) => result._exception ?? throw new InvalidOperationException();
    
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
    
    public void Match(Action onSuccess, Action<Exception> onFailure)
    {
        if (IsSuccess)
        {
            onSuccess();
        }
        else
        {
            onFailure(Exception);
        }
    }
}
