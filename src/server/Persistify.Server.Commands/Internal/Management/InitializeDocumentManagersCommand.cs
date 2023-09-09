using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Shared;
using Persistify.Responses.Shared;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Internal.Management;

// TODO: Refactor to use transaction promotion
public class InitializeDocumentManagersCommand : Command
{
    private readonly IDocumentManagerStore _documentManagerStore;

    public InitializeDocumentManagersCommand(
        ILoggerFactory loggerFactory,
        IDocumentManagerStore documentManagerStore,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler
    ) : base(
        loggerFactory,
        transactionState,
        exceptionHandler
    )
    {
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(EmptyRequest data, CancellationToken cancellationToken)
    {
        var documentManagers = _documentManagerStore.GetManagers().ToList();

        foreach (var documentManager in documentManagers)
        {
            await TransactionState.GetCurrentTransaction().PromoteManagerAsync(documentManager, true, TransactionTimeout);
            documentManager.Initialize();
        }
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
            new List<IManager>()
        );
    }
}
