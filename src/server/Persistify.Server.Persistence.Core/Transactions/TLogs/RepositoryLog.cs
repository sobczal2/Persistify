using System;
using ProtoBuf;

namespace Persistify.Server.Persistence.Core.Transactions.TLogs;

[ProtoContract]
public class RepositoryLog : TransactionLog
{
    [ProtoMember(102)]
    public RepositoryType RepositoryType { get; set; }
    [ProtoMember(103)]
    public int Key { get; set; }
    [ProtoMember(104)]
    public RepositoryAction RepositoryAction { get; }

    public RepositoryLog(RepositoryType repositoryType, int key, RepositoryAction repositoryAction, DateTime timestamp) : base(timestamp)
    {
        RepositoryType = repositoryType;
        Key = key;
        RepositoryAction = repositoryAction;
    }

    public override string ToString()
    {
        return $"{base.ToString()} {RepositoryType} {RepositoryAction} {Key}";
    }
}
