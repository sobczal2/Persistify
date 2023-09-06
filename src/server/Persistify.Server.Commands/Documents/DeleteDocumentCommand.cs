using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Commands.Common;
using Persistify.Server.Errors;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Commands.Documents;

public class DeleteDocumentCommand : Command<DeleteDocumentRequest, DeleteDocumentResponse>
{
    private readonly ITemplateManager _templateManager;
    private readonly IDocumentManagerStore _documentManagerStore;

    public DeleteDocumentCommand(
        IValidator<DeleteDocumentRequest> validator,
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        ITemplateManager templateManager,
        IDocumentManagerStore documentManagerStore
    ) : base(
        validator,
        loggerFactory,
        transactionState
    )
    {
        _templateManager = templateManager;
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(DeleteDocumentRequest data, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(data.TemplateId);

        if (template is null)
        {
            throw new ValidationException(nameof(data.TemplateId), $"Template with id {data.TemplateId} not found");
        }

        var documentManager = _documentManagerStore.GetManager(template.Id);

        if (documentManager is null)
        {
            throw new PersistifyInternalException();
        }

        await TransactionState.GetCurrentTransaction().PromoteManagerAsync(documentManager, true, TransactionTimeout);

        var result = await documentManager.RemoveAsync(data.DocumentId);

        if (!result)
        {
            throw new ValidationException(nameof(data.DocumentId), $"Document with id {data.DocumentId} not found");
        }
    }

    protected override DeleteDocumentResponse GetResponse()
    {
        return new DeleteDocumentResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(DeleteDocumentRequest data)
    {
        return new TransactionDescriptor(
            exclusiveGlobal: false,
            readManagers: new List<IManager> { _templateManager },
            writeManagers: new List<IManager>()
        );
    }
}
