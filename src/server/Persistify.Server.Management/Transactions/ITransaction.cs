using System;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.Management.Managers;

namespace Persistify.Server.Management.Transactions;

public interface ITransaction
{
    Guid Id { get; }
    TransactionPhase Phase { get; }

    ValueTask BeginAsync(
        TimeSpan timeOut,
        CancellationToken cancellationToken
    );

    ValueTask PromoteManagerAsync(
        IManager manager,
        bool write,
        TimeSpan timeOut
    );

    ValueTask CommitAsync();
    ValueTask RollbackAsync();
}
