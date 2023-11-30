using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.Internal;
using Persistify.Responses.Internal;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Users;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Internal;

public class InitializeDocumentManagersRequestHandler
    : RequestHandler<InitializeDocumentManagersRequest, InitializeDocumentManagersResponse>
{
    private readonly IDocumentManagerStore _documentManagerStore;

    public InitializeDocumentManagersRequestHandler(
        IRequestHandlerContext<
            InitializeDocumentManagersRequest,
            InitializeDocumentManagersResponse
        > requestHandlerContext,
        IDocumentManagerStore documentManagerStore
    )
        : base(requestHandlerContext)
    {
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(
        InitializeDocumentManagersRequest request,
        CancellationToken cancellationToken
    )
    {
        var documentManagers = _documentManagerStore.GetManagers().ToList();

        foreach (var documentManager in documentManagers)
        {
            await RequestHandlerContext.CurrentTransaction.PromoteManagerAsync(
                documentManager,
                true,
                TransactionTimeout
            );
            documentManager.Initialize();
        }
    }

    protected override InitializeDocumentManagersResponse GetResponse()
    {
        return new InitializeDocumentManagersResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(
        InitializeDocumentManagersRequest request
    )
    {
        return new TransactionDescriptor(false, new List<IManager>(), new List<IManager>());
    }

    protected override Permission GetRequiredPermission(
        InitializeDocumentManagersRequest request
    )
    {
        return Permission.DocumentWrite;
    }
}
