using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Dtos.Documents.Common;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Documents;
using Persistify.Server.Domain.Users;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Documents;

public class GetDocumentRequestHandler : RequestHandler<GetDocumentRequest, GetDocumentResponse>
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly ITemplateManager _templateManager;
    private Document? _document;

    public GetDocumentRequestHandler(
        IRequestHandlerContext<GetDocumentRequest, GetDocumentResponse> requestHandlerContext,
        ITemplateManager templateManager,
        IDocumentManagerStore documentManagerStore
    ) : base(
        requestHandlerContext
    )
    {
        _templateManager = templateManager;
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(GetDocumentRequest request, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(request.TemplateName) ??
                       throw new InternalPersistifyException(nameof(GetDocumentRequest));

        var documentManager = _documentManagerStore.GetManager(template.Id) ??
                              throw new InternalPersistifyException(nameof(GetDocumentRequest));

        await RequestHandlerContext
            .CurrentTransaction
            .PromoteManagerAsync(documentManager, true, TransactionTimeout);

        _document = await documentManager.GetAsync(request.DocumentId);

        if (_document is null)
        {
            throw new DynamicValidationPersistifyException(nameof(GetDocumentRequest.DocumentId),
                DocumentErrorMessages.InvalidDocumentId);
        }
    }

    protected override GetDocumentResponse GetResponse()
    {
        var document = _document ?? throw new InternalPersistifyException(nameof(GetDocumentRequest));
        return new GetDocumentResponse
        {
            Document = new DocumentDto
            {
                Id = document.Id,
                FieldValues = document.FieldValues.Select(x =>
                {
                    FieldValueDto fieldValueDto = x switch
                    {
                        BoolFieldValue boolFieldValue => new BoolFieldValueDto
                        {
                            FieldName = boolFieldValue.FieldName, Value = boolFieldValue.Value
                        },
                        NumberFieldValue numberFieldValue => new NumberFieldValueDto
                        {
                            FieldName = numberFieldValue.FieldName, Value = numberFieldValue.Value
                        },
                        TextFieldValue textFieldValue => new TextFieldValueDto
                        {
                            FieldName = textFieldValue.FieldName, Value = textFieldValue.Value
                        },
                        _ => throw new InternalPersistifyException(nameof(GetDocumentRequest))
                    };

                    return fieldValueDto;
                }).ToList()
            }
        };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(GetDocumentRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _templateManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(GetDocumentRequest request)
    {
        return Permission.DocumentRead | Permission.TemplateRead;
    }
}
