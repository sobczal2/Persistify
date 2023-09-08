using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Shared;
using Persistify.Responses.Shared;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Commands.Common;

public abstract class Command<TData, TResponse>
{
    private readonly IValidator<TData> _validator;
    protected readonly ILoggerFactory LoggerFactory;

    protected readonly ITransactionState TransactionState;

    public Command(
        IValidator<TData> validator,
        ILoggerFactory loggerFactory,
        ITransactionState transactionState
    )
    {
        _validator = validator;
        LoggerFactory = loggerFactory;
        TransactionState = transactionState;
    }

    // TODO: move to config
    protected TimeSpan TransactionTimeout => TimeSpan.FromSeconds(30);

    protected abstract ValueTask RunAsync(TData data, CancellationToken cancellationToken);
    protected abstract TResponse GetResponse();
    protected abstract TransactionDescriptor GetTransactionDescriptor(TData data);

    public async ValueTask<TResponse> RunInTransactionAsync(TData data, CancellationToken cancellationToken)
    {
        _validator.Validate(data).OnFailure(exception => throw exception);

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
        catch (Exception)
        {
            await transaction.RollbackAsync().ConfigureAwait(false);
            throw;
        }
    }
}

public abstract class Command : Command<EmptyRequest, EmptyResponse>
{
    protected Command(
        ILoggerFactory loggerFactory,
        ITransactionState transactionState
    ) : base(
        new EmptyRequestValidator(),
        loggerFactory,
        transactionState
    )
    {
    }
}
