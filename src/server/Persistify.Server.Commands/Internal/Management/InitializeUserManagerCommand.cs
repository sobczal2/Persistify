using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Shared;
using Persistify.Responses.Shared;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Internal.Management;

public class InitializeUserManagerCommand : Command
{
    private readonly IUserManager _userManager;

    public InitializeUserManagerCommand(
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler,
        IUserManager userManager
    ) : base(
        loggerFactory,
        transactionState,
        exceptionHandler
    )
    {
        _userManager = userManager;
    }

    protected override ValueTask RunAsync(EmptyRequest data, CancellationToken cancellationToken)
    {
        _userManager.Initialize();

        return ValueTask.CompletedTask;
    }

    protected override EmptyResponse GetResponse()
    {
        return new EmptyResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(EmptyRequest data)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }
}
