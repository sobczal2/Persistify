using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Documents;

public class SearchDocumentsCommand : Command<SearchDocumentsRequest, SearchDocumentsResponse>
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly ITemplateManager _templateManager;
    private SearchDocumentsResponse? _response;

    public SearchDocumentsCommand(
        ICommandContext<SearchDocumentsRequest> commandContext,
        IDocumentManagerStore documentManagerStore,
        ITemplateManager templateManager
    ) : base(
        commandContext
    )
    {
        _documentManagerStore = documentManagerStore;
        _templateManager = templateManager;
    }

    protected override async ValueTask RunAsync(SearchDocumentsRequest request, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(request.TemplateName) ?? throw new PersistifyInternalException();

        var skip = request.Pagination.PageNumber * request.Pagination.PageSize;
        var take = request.Pagination.PageSize;

        var documentManager = _documentManagerStore.GetManager(template.Id) ?? throw new PersistifyInternalException();

        await CommandContext.CurrentTransaction
            .PromoteManagerAsync(documentManager, true, TransactionTimeout);

        var (documents, count) = await documentManager.SearchAsync(request.SearchQuery, take, skip);

        _response = new SearchDocumentsResponse(documents, count);
    }

    protected override SearchDocumentsResponse GetResponse()
    {
        return _response ?? throw new PersistifyInternalException();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(SearchDocumentsRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _templateManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(SearchDocumentsRequest request)
    {
        return Permission.DocumentRead;
    }
}
