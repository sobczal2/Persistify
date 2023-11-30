using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Persistify.Server.Management.Managers;

namespace Persistify.Server.Management.Transactions;

public class TransactionDescriptor : ITransactionDescriptor
{
    private readonly List<IManager> _readManagers;
    private readonly List<IManager> _writeManagers;

    public TransactionDescriptor(
        bool exclusiveGlobal,
        List<IManager> readManagers,
        List<IManager> writeManagers
    )
    {
        ExclusiveGlobal = exclusiveGlobal;
        _readManagers = readManagers ?? throw new ArgumentNullException(nameof(readManagers));
        _writeManagers = writeManagers ?? throw new ArgumentNullException(nameof(writeManagers));
    }

    public bool ExclusiveGlobal { get; }
    public IImmutableList<IManager> ReadManagers => _readManagers.ToImmutableList();
    public IImmutableList<IManager> WriteManagers => _writeManagers.ToImmutableList();

    public void AddReadManager(
        IManager manager
    )
    {
        if (manager is null)
        {
            throw new ArgumentNullException(nameof(manager));
        }

        _readManagers.Add(manager);
    }

    public void AddWriteManager(
        IManager manager
    )
    {
        if (manager is null)
        {
            throw new ArgumentNullException(nameof(manager));
        }

        _writeManagers.Add(manager);
    }
}
