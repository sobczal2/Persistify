using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Common;
using Persistify.Responses.Common;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Common;

public interface IRequestHandlerContext<in TRequest, out TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResponse
{
    ITransaction CurrentTransaction { get; }
    ITransactionState TransactionState { get; }
    ValueTask ValidateAsync(TRequest request);
    ILogger<T> CreateLogger<T>();
    void HandleException(Exception exception);
}
