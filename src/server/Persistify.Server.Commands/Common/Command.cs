using System;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Commands.Common;

public abstract class Command<TData, TResponse>
{
    private readonly IValidator<TData> _validator;
    private static TimeSpan TransactionTimeout => TimeSpan.FromSeconds(30);

    public Command(IValidator<TData> validator)
    {
        _validator = validator;
    }

    protected abstract ValueTask ExecuteAsync(TData data, CancellationToken cancellationToken);
    protected abstract TResponse? Response { get; }
    protected abstract TransactionDescriptor TransactionDescriptor { get; }

    public async ValueTask<TResponse> RunInTransactionAsync(TData data, CancellationToken cancellationToken)
    {
        var result = _validator.Validate(data);
        if (result.Failure)
        {
            result.ThrowSuppressed();
        }

        var transaction = new Transaction(TransactionDescriptor);
        Transaction.CurrentTransaction.Value = transaction;
        await transaction.BeginAsync(TransactionTimeout, cancellationToken).ConfigureAwait(false);
        try
        {
            await ExecuteAsync(data, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
            return Response ?? throw new InvalidOperationException();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync().ConfigureAwait(false);
            throw;
        }
    }
}
