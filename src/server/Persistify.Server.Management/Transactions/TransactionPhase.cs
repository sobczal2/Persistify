namespace Persistify.Server.Management.Transactions;

public enum TransactionPhase
{
    Ready,
    Started,
    Committed,
    RolledBack
}
