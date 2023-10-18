using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Documents;

public class DeleteDocumentRequestHandler : RequestHandler<DeleteDocumentRequest, DeleteDocumentResponse>
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly ITemplateManager _templateManager;

    public DeleteDocumentRequestHandler(
        IRequestHandlerContext<DeleteDocumentRequest, DeleteDocumentResponse> requestHandlerContext,
        ITemplateManager templateManager,
        IDocumentManagerStore documentManagerStore
    ) : base(
        requestHandlerContext
    )
    {
        _templateManager = templateManager;
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(DeleteDocumentRequest request, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(request.TemplateName) ??
                       throw new NotFoundPersistifyException(nameof(DeleteDocumentRequest), DocumentErrorMessages.TemplateNotFound);

        var documentManager = _documentManagerStore.GetManager(template.Id) ??
                              throw new InternalPersistifyException(nameof(DeleteDocumentRequest));

        await RequestHandlerContext
            .CurrentTransaction
            .PromoteManagerAsync(documentManager, true, TransactionTimeout);

        var result = await documentManager.RemoveAsync(request.DocumentId);

        if (!result)
        {
            throw new NotFoundPersistifyException(nameof(DeleteDocumentRequest), DocumentErrorMessages.DocumentNotFound);
        }
    }

    protected override DeleteDocumentResponse GetResponse()
    {
        return new DeleteDocumentResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(DeleteDocumentRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _templateManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(DeleteDocumentRequest request)
    {
        return Permission.DocumentWrite | Permission.TemplateRead;
    }
}
