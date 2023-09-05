using System.Collections.Generic;
using System.Collections.Immutable;
using Persistify.Server.Management.Managers;

namespace Persistify.Server.Management.Transactions;

public class TransactionDescriptor
{
    public bool ExclusiveGlobal { get; }

    public List<IManager> ReadManagers { get; }
    public List<IManager> WriteManagers { get; }

    public TransactionDescriptor(
        bool exclusiveGlobal,
        List<IManager> readManagers,
        List<IManager> writeManagers
    )
    {
        ExclusiveGlobal = exclusiveGlobal;
        ReadManagers = readManagers;
        WriteManagers = writeManagers;
    }
}
