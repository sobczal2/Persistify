using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Shared;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Common;

public interface ICommandContext<in TRequest>
{
    ITransaction CurrentTransaction { get; }
    ITransactionState TransactionState { get; }
    ValueTask ValidateAsync(TRequest request);
    ILogger<T> CreateLogger<T>();
    void HandleException(Exception exception);
}

public interface ICommandContext : ICommandContext<EmptyRequest>
{
}
