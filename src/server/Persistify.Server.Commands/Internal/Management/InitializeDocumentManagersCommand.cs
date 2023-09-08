using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Shared;
using Persistify.Responses.Shared;
using Persistify.Server.Commands.Common;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Internal.Management;

// TODO: Refactor to use transaction promotion
public class InitializeDocumentManagersCommand : Command
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private IEnumerable<IDocumentManager> _documentManagers;

    public InitializeDocumentManagersCommand(
        ILoggerFactory loggerFactory,
        IDocumentManagerStore documentManagerStore,
        ITransactionState transactionState
    ) : base(
        loggerFactory,
        transactionState
    )
    {
        _documentManagerStore = documentManagerStore;
        _documentManagers = Enumerable.Empty<IDocumentManager>();
    }

    protected override ValueTask RunAsync(EmptyRequest data, CancellationToken cancellationToken)
    {
        foreach (var documentManager in _documentManagers)
        {
            documentManager.Initialize();
        }

        return ValueTask.CompletedTask;
    }

    protected override EmptyResponse GetResponse()
    {
        return new EmptyResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(EmptyRequest data)
    {
        _documentManagers = _documentManagerStore.GetManagers();

        var managers = _documentManagers.Cast<IManager>().ToList();
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            managers
        );
    }
}
