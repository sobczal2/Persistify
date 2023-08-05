using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Core.Transactions.TLogs;
using ProtoBuf;

namespace Persistify.Server.Persistence.Core.Transactions;

[ProtoContract]
public class Transaction
{
    [ProtoMember(1)]
    public long Id { get; set; }
    [ProtoMember(2)]
    public bool WriteGlobal { get; set; }
    [ProtoMember(3)]
    public Dictionary<int, bool> TemplateWriteMap { get; set; }
    public Stack<Func<ValueTask>> RollbackActions { get; set; }
    [ProtoMember(4)]
    public List<TransactionLog> TransactionLogs { get; set; }
    [ProtoMember(5)]
    public DateTime? StartTimestamp { get; set; }
    [ProtoMember(6)]
    public DateTime? EndTimestamp { get; set; }
    [ProtoMember(7)]
    public bool RolledBack { get; set; }
    [ProtoMember(8)]
    public bool Committed { get; set; }

    public Transaction(bool writeGlobal, Dictionary<int, bool> templateWriteMap)
    {
        Id = 0;
        WriteGlobal = writeGlobal;
        TemplateWriteMap = templateWriteMap;
        RollbackActions = new Stack<Func<ValueTask>>();
        TransactionLogs = new List<TransactionLog>();
        StartTimestamp = null;
        EndTimestamp = null;
        RolledBack = false;
        Committed = false;
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Transaction {Id}");
        sb.AppendLine($"\tStart: {StartTimestamp?.ToString("O") ?? "null"}");
        sb.AppendLine($"\tEnd: {EndTimestamp?.ToString("O") ?? "null"}");
        sb.AppendLine($"\tRolledBack: {RolledBack}");
        sb.AppendLine($"\tCommitted: {Committed}");
        sb.AppendLine($"\tWrite: {WriteGlobal}");
        sb.AppendLine($"\tTemplateWriteMap: {TemplateWriteMap.Count}");
        sb.AppendLine($"\tTemplateWriteMap: \n\t\t{string.Join("\n\t\t", TemplateWriteMap)}");
        sb.AppendLine($"\tRollbackActions: {RollbackActions.Count}");
        sb.AppendLine($"\tTransactionLogs: {TransactionLogs.Count}");
        sb.AppendLine($"\tTransactionLogs: \n\t\t{string.Join("\n\t\t", TransactionLogs)}");
        return sb.ToString();
    }
}
