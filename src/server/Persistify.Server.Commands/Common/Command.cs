using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Shared;
using Persistify.Responses.Shared;
using Persistify.Server.ErrorHandling;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Commands.Common;

public abstract class Command<TData, TResponse>
{
    private readonly IValidator<TData> _validator;
    protected readonly ILoggerFactory LoggerFactory;

    protected readonly ITransactionState TransactionState;
    private readonly IExceptionHandler _exceptionHandler;

    public Command(
        IValidator<TData> validator,
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler
    )
    {
        _validator = validator;
        LoggerFactory = loggerFactory;
        TransactionState = transactionState;
        _exceptionHandler = exceptionHandler;
    }

    // TODO: move to config
    protected TimeSpan TransactionTimeout => TimeSpan.FromSeconds(30);

    protected abstract ValueTask RunAsync(TData data, CancellationToken cancellationToken);
    protected abstract TResponse GetResponse();
    protected abstract TransactionDescriptor GetTransactionDescriptor(TData data);

    public async ValueTask<TResponse> RunInTransactionAsync(TData data, CancellationToken cancellationToken)
    {
        _validator.Validate(data).OnFailure(exception => _exceptionHandler.Handle(exception));

        var transactionDescriptor = GetTransactionDescriptor(data);

        var transaction = new Transaction(
            transactionDescriptor,
            TransactionState,
            LoggerFactory.CreateLogger<Transaction>()
        );

        TransactionState.CurrentTransaction.Value = transaction;

        await transaction.BeginAsync(TransactionTimeout, cancellationToken)
            .ConfigureAwait(false);
        try
        {
            await RunAsync(data, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
            return GetResponse();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync().ConfigureAwait(false);
            _exceptionHandler.Handle(exception);
        }

        throw new PersistifyInternalException();
    }
}

public abstract class Command : Command<EmptyRequest, EmptyResponse>
{
    protected Command(
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler
    ) : base(
        new EmptyRequestValidator(),
        loggerFactory,
        transactionState,
        exceptionHandler
    )
    {
    }
}
