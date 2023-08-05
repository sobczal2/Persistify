using System;
using ProtoBuf;

namespace Persistify.Server.Persistence.Core.Transactions.TLogs;

[ProtoContract]
public class PipelineStageLog : TransactionLog
{
    [ProtoMember(105)]
    public string PipelineStageName { get; set; }
    [ProtoMember(106)]
    public PipelineStageState State { get; set; }
    public PipelineStageLog(string pipelineStageName, PipelineStageState state, DateTime timestamp) : base(timestamp)
    {
        PipelineStageName = pipelineStageName;
        State = state;
    }

    public override string ToString()
    {
        return $"{base.ToString()} {PipelineStageName} {State}";
    }
}
