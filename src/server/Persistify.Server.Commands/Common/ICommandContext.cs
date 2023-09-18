using System;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Shared;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Common;

public interface ICommandContext<in TRequest>
{
    ITransaction CurrentTransaction { get; }
    void Validate(TRequest request);
    ILogger<T> CreateLogger<T>();
    ITransactionState TransactionState { get; }
    void HandleException(Exception exception);
}

public interface ICommandContext : ICommandContext<EmptyRequest>
{
}
