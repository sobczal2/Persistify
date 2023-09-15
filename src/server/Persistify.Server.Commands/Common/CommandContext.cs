using System;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Shared;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Commands.Common;

public class CommandContext<TRequest> : ICommandContext<TRequest>
{
    private readonly IValidator<TRequest> _validator;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IExceptionHandler _exceptionHandler;
    public ITransactionState TransactionState { get; }

    public CommandContext(
        IValidator<TRequest> validator,
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler
    )
    {
        _validator = validator;
        _loggerFactory = loggerFactory;
        TransactionState = transactionState;
        _exceptionHandler = exceptionHandler;
    }

    public void HandleException(Exception exception)
    {
        _exceptionHandler.Handle(exception);
    }

    public ITransaction CurrentTransaction => TransactionState.GetCurrentTransaction();

    public void Validate(TRequest request)
    {
        _validator.Validate(request).OnFailure(ex => _exceptionHandler.Handle(ex));
    }

    public ILogger<T> CreateLogger<T>()
    {
        return _loggerFactory.CreateLogger<T>();
    }
}

public class CommandContext : CommandContext<EmptyRequest>, ICommandContext
{
    public CommandContext
    (IValidator<EmptyRequest> validator,
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler
    ) : base(
        validator,
        loggerFactory,
        transactionState,
        exceptionHandler
    )
    {
    }
}
