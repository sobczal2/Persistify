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
using Persistify.Server.Validation.Documents;

namespace Persistify.Server.Commands.Documents;

public class DeleteDocumentCommand : Command<DeleteDocumentRequest, DeleteDocumentResponse>
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly ITemplateManager _templateManager;

    public DeleteDocumentCommand(
        ICommandContext<DeleteDocumentRequest> commandContext,
        ITemplateManager templateManager,
        IDocumentManagerStore documentManagerStore
    ) : base(
        commandContext
    )
    {
        _templateManager = templateManager;
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(DeleteDocumentRequest request, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(request.TemplateName) ?? throw new PersistifyInternalException();

        var documentManager = _documentManagerStore.GetManager(template.Id) ?? throw new PersistifyInternalException();

        await CommandContext
            .CurrentTransaction
            .PromoteManagerAsync(documentManager, true, TransactionTimeout);

        var result = await documentManager.RemoveAsync(request.DocumentId);

        if (!result)
        {
            throw ValidationException(nameof(DeleteDocumentRequest.DocumentId),
                DocumentErrorMessages.InvalidDocumentId);
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
        return Permission.DocumentWrite;
    }
}
