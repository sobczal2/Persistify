using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Shared;
using Persistify.Responses.Shared;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Security;

namespace Persistify.Server.Commands.Common;

public abstract class Command<TRequest, TResponse>
{
    protected readonly ICommandContext<TRequest> CommandContext;

    public Command(
        ICommandContext<TRequest> commandContext
    )
    {
        CommandContext = commandContext;
    }

    // TODO: move to config
    protected TimeSpan TransactionTimeout => TimeSpan.FromSeconds(30);

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

    public async ValueTask<TResponse> RunInTransactionAsync(TRequest request, ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        Authorize(claimsPrincipal, request);

        var transactionDescriptor = GetTransactionDescriptor(request);

        var transaction = new Transaction(
            transactionDescriptor,
            CommandContext.TransactionState,
            CommandContext.CreateLogger<Transaction>()
        );

        CommandContext.TransactionState.CurrentTransaction.Value = transaction;

        await transaction.BeginAsync(TransactionTimeout, cancellationToken)
            .ConfigureAwait(false);
        try
        {
            await CommandContext.ValidateAsync(request).ConfigureAwait(false);
            await RunAsync(request, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
            return GetResponse();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync().ConfigureAwait(false);
            CommandContext.HandleException(exception);
        }

        throw new InternalPersistifyException(request?.GetType().Name);
    }
}

public abstract class Command : Command<EmptyRequest, EmptyResponse>
{
    protected Command(
        ICommandContext commandContext
    ) : base(
        commandContext
    )
    {
    }

    protected override EmptyResponse GetResponse()
    {
        return new EmptyResponse();
    }
}
