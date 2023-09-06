using System;
using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Server.Management.Transactions;

public interface ITransactionState
{
    AsyncLocal<ITransaction?> CurrentTransaction { get; }
    ITransaction GetCurrentTransaction();
    ValueTask EnterReadGlobalLockAsync(Guid transactionId, TimeSpan timeOut, CancellationToken cancellationToken);
    ValueTask EnterWriteGlobalLockAsync(Guid transactionId, TimeSpan timeOut, CancellationToken cancellationToken);
    ValueTask ExitReadGlobalLockAsync(Guid transactionId);
    ValueTask ExitWriteGlobalLockAsync(Guid transactionId);
}
