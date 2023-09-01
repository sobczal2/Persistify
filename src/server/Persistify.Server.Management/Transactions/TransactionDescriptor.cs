using System.Collections.Generic;
using System.Collections.Immutable;
using Persistify.Server.Management.Managers;

namespace Persistify.Server.Management.Transactions;

public class TransactionDescriptor
{
    public bool ExclusiveGlobal { get; }

    public IImmutableList<IManager> ReadManagers { get; }
    public IImmutableList<IManager> WriteManagers { get; }

    public TransactionDescriptor(
        bool exclusiveGlobal,
        IEnumerable<IManager> readManagers,
        IEnumerable<IManager> writeManagers
    )
    {
        ExclusiveGlobal = exclusiveGlobal;
        ReadManagers = readManagers.ToImmutableList();
        WriteManagers = writeManagers.ToImmutableList();
    }
}
