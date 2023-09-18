using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
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

public class GetDocumentCommand : Command<GetDocumentRequest, GetDocumentResponse>
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly ITemplateManager _templateManager;
    private Document? _document;

    public GetDocumentCommand(
        ICommandContext<GetDocumentRequest> commandContext,
        ITemplateManager templateManager,
        IDocumentManagerStore documentManagerStore
    ) : base(
        commandContext
    )
    {
        _templateManager = templateManager;
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(GetDocumentRequest request, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(request.TemplateName) ?? throw new PersistifyInternalException();

        var documentManager = _documentManagerStore.GetManager(template.Id) ?? throw new PersistifyInternalException();

        await CommandContext
            .CurrentTransaction
            .PromoteManagerAsync(documentManager, true, TransactionTimeout);

        _document = await documentManager.GetAsync(request.DocumentId);

        if (_document is null)
        {
            throw ValidationException(nameof(GetDocumentRequest.DocumentId), DocumentErrorMessages.InvalidDocumentId);
        }
    }

    protected override GetDocumentResponse GetResponse()
    {
        return new GetDocumentResponse(_document ?? throw new PersistifyInternalException());
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
        return Permission.DocumentRead;
    }
}
