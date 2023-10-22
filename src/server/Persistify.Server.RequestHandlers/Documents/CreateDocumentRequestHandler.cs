using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Documents;
using Persistify.Server.Domain.Users;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Mappers.Documents;

namespace Persistify.Server.CommandHandlers.Documents;

public sealed class CreateDocumentRequestHandler : RequestHandler<CreateDocumentRequest, CreateDocumentResponse>
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly ITemplateManager _templateManager;
    private Document? _document;

    public CreateDocumentRequestHandler(
        IRequestHandlerContext<CreateDocumentRequest, CreateDocumentResponse> requestHandlerContext,
        ITemplateManager templateManager,
        IDocumentManagerStore documentManagerStore
    ) : base(
        requestHandlerContext
    )
    {
        _templateManager = templateManager;
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(CreateDocumentRequest request, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(request.TemplateName) ??
                       throw new InternalPersistifyException(nameof(CreateDocumentRequest));

        var fieldValues = new List<FieldValue>();

        foreach (var fieldValueDto in request.FieldValueDtos)
        {
            fieldValues.Add(fieldValueDto.ToDomain());
        }

        _document = new Document { FieldValues = fieldValues };

        var documentManager = _documentManagerStore.GetManager(template.Id) ??
                              throw new InternalPersistifyException(nameof(CreateDocumentRequest));

        await RequestHandlerContext.CurrentTransaction
            .PromoteManagerAsync(documentManager, true, TransactionTimeout);

        documentManager.Add(_document);
    }

    protected override CreateDocumentResponse GetResponse()
    {
        return new CreateDocumentResponse
        {
            DocumentId = _document?.Id ?? throw new InternalPersistifyException(nameof(CreateDocumentRequest))
        };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(CreateDocumentRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _templateManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(CreateDocumentRequest request)
    {
        return Permission.DocumentWrite | Permission.TemplateRead;
    }
}
