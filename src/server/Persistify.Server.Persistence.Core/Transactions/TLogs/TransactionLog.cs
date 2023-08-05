using System;
using ProtoBuf;

namespace Persistify.Server.Persistence.Core.Transactions.TLogs;

[ProtoContract]
[ProtoInclude(1, typeof(RepositoryLog))]
[ProtoInclude(2, typeof(PipelineStageLog))]
public abstract class TransactionLog
{
    [ProtoMember(101)]
    public DateTime Timestamp { get; set; }

    protected TransactionLog(DateTime timestamp)
    {
        Timestamp = timestamp;
    }

    public override string ToString()
    {
        return $"[{Timestamp:O}]";
    }
}
