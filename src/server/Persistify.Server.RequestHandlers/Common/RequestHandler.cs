using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.Common;
using Persistify.Responses.Common;
using Persistify.Server.Domain.Users;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Security;

namespace Persistify.Server.CommandHandlers.Common;

public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResponse
{
    protected readonly IRequestHandlerContext<TRequest, TResponse> RequestHandlerContext;

    public RequestHandler(
        IRequestHandlerContext<TRequest, TResponse> requestHandlerContext
    )
    {
        RequestHandlerContext = requestHandlerContext;
    }

    // TODO: move to config
    protected TimeSpan TransactionTimeout => TimeSpan.FromSeconds(30);

    public async ValueTask<TResponse> HandleAsync(TRequest request, ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        Authorize(claimsPrincipal, request);

        var transactionDescriptor = GetTransactionDescriptor(request);

        var transaction = new Transaction(
            transactionDescriptor,
            RequestHandlerContext.TransactionState,
            RequestHandlerContext.CreateLogger<Transaction>()
        );

        RequestHandlerContext.TransactionState.CurrentTransaction.Value = transaction;

        await transaction.BeginAsync(TransactionTimeout, cancellationToken)
            .ConfigureAwait(false);
        try
        {
            await RequestHandlerContext.ValidateAsync(request).ConfigureAwait(false);
            await RunAsync(request, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync().ConfigureAwait(false);
            RequestHandlerContext.HandleException(exception);
        }
        finally
        {
            RequestHandlerContext.TransactionState.CurrentTransaction.Value = null;
        }

        return GetResponse();
    }

    protected abstract ValueTask RunAsync(TRequest request, CancellationToken cancellationToken);
    protected abstract TResponse GetResponse();
    protected abstract TransactionDescriptor GetTransactionDescriptor(TRequest request);
    protected abstract Permission GetRequiredPermission(TRequest request);

    protected virtual void Authorize(ClaimsPrincipal claimsPrincipal, TRequest request)
    {
        var requiredPermission = GetRequiredPermission(request);
        var userPermission = claimsPrincipal.GetPermission();
        if (!userPermission.HasFlag(requiredPermission))
        {
            throw new InsufficientPermissionPersistifyException(request?.GetType().Name, requiredPermission);
        }
    }
}
