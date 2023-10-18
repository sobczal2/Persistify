using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Documents;

public class SearchDocumentsRequestHandler : RequestHandler<SearchDocumentsRequest, SearchDocumentsResponse>
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly ITemplateManager _templateManager;
    private SearchDocumentsResponse? _response;

    public SearchDocumentsRequestHandler(
        IRequestHandlerContext<SearchDocumentsRequest, SearchDocumentsResponse> requestHandlerContext,
        IDocumentManagerStore documentManagerStore,
        ITemplateManager templateManager
    ) : base(
        requestHandlerContext
    )
    {
        _documentManagerStore = documentManagerStore;
        _templateManager = templateManager;
    }

    protected override async ValueTask RunAsync(SearchDocumentsRequest request, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(request.TemplateName) ??
                       throw new InternalPersistifyException(nameof(SearchDocumentsRequest));

        var skip = request.Pagination.PageNumber * request.Pagination.PageSize;
        var take = request.Pagination.PageSize;

        var documentManager = _documentManagerStore.GetManager(template.Id) ??
                              throw new InternalPersistifyException(nameof(SearchDocumentsRequest));

        await RequestHandlerContext.CurrentTransaction
            .PromoteManagerAsync(documentManager, true, TransactionTimeout);

        var (searchRecords, count) = await documentManager.SearchAsync(request.SearchQueryDto, take, skip);

        _response = new SearchDocumentsResponse { SearchRecords = searchRecords, TotalCount = count };
    }

    protected override SearchDocumentsResponse GetResponse()
    {
        return _response ?? throw new InternalPersistifyException(nameof(SearchDocumentsRequest));
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
        return Permission.DocumentRead | Permission.TemplateRead;
    }
}
