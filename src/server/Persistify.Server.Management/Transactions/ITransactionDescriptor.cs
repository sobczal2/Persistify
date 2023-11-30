using System.Collections.Immutable;
using Persistify.Server.Management.Managers;

namespace Persistify.Server.Management.Transactions;

public interface ITransactionDescriptor
{
    bool ExclusiveGlobal { get; }
    IImmutableList<IManager> ReadManagers { get; }
    IImmutableList<IManager> WriteManagers { get; }

    void AddReadManager(
        IManager manager
    );

    void AddWriteManager(
        IManager manager
    );
}
