using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Server.Errors;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Commands.Common;

public abstract class Command<TData, TResponse>
{
    private readonly IValidator<TData> _validator;
    protected readonly ILoggerFactory LoggerFactory;
    private static TimeSpan TransactionTimeout => TimeSpan.FromSeconds(30);

    public Command(
        IValidator<TData> validator,
        ILoggerFactory loggerFactory
        )
    {
        _validator = validator;
        LoggerFactory = loggerFactory;
    }

    protected abstract ValueTask RunAsync(TData data, CancellationToken cancellationToken);
    protected abstract TResponse GetResponse();
    protected abstract TransactionDescriptor GetTransactionDescriptor(TData data);

    public async ValueTask<TResponse> RunInTransactionAsync(TData data, CancellationToken cancellationToken)
    {
        _validator.Validate(data).OnFailure(exception => throw exception);

        var transactionDescriptor = GetTransactionDescriptor(data);

        var transaction = new Transaction(transactionDescriptor, LoggerFactory.CreateLogger<Transaction>());
        Transaction.CurrentTransaction.Value = transaction;
        await transaction.BeginAsync(TransactionTimeout, cancellationToken).ConfigureAwait(false);
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
