using System;
using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Repositories;

public interface IRepository
{
    ValueTask<bool> BeginReadAsync(TimeSpan timeOut, CancellationToken cancellationToken);
    ValueTask<bool> BeginWriteAsync(TimeSpan timeOut, CancellationToken cancellationToken);

    ValueTask EndReadAsync();
    ValueTask EndWriteAsync();

    ValueTask ExecutePendingActionsAsync();
    void ClearPendingActions();
}
