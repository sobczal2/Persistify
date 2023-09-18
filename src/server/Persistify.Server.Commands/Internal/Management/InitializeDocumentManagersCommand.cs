using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Shared;
using Persistify.Server.Commands.Common;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Internal.Management;

// TODO: Refactor to use transaction promotion
public class InitializeDocumentManagersCommand : Command
{
    private readonly IDocumentManagerStore _documentManagerStore;

    public InitializeDocumentManagersCommand(
        ICommandContext commandContext,
        IDocumentManagerStore documentManagerStore
    ) : base(
        commandContext
    )
    {
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(EmptyRequest request, CancellationToken cancellationToken)
    {
        var documentManagers = _documentManagerStore.GetManagers().ToList();

        foreach (var documentManager in documentManagers)
        {
            await CommandContext
                .CurrentTransaction
                .PromoteManagerAsync(documentManager, true, TransactionTimeout);
            documentManager.Initialize();
        }
    }

    protected override TransactionDescriptor GetTransactionDescriptor(EmptyRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(EmptyRequest request)
    {
        return Permission.DocumentWrite;
    }
}
