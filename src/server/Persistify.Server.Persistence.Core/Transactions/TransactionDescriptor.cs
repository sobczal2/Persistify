using System.Collections.Generic;
using System.Collections.Immutable;
using Persistify.Server.Persistence.Core.Repositories;

namespace Persistify.Server.Persistence.Core.Transactions;

public class TransactionDescriptor
{
    public bool ExclusiveGlobal { get; }

    public IImmutableList<IRepository> ReadRepositories { get; }
    public IImmutableList<IRepository> WriteRepositories { get; }

    public TransactionDescriptor(
        bool exclusiveGlobal,
        IEnumerable<IRepository> readRepositories,
        IEnumerable<IRepository> writeRepositories
    )
    {
        ExclusiveGlobal = exclusiveGlobal;
        ReadRepositories = readRepositories.ToImmutableList();
        WriteRepositories = writeRepositories.ToImmutableList();
    }
}
