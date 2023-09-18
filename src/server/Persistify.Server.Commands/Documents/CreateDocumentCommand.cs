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

namespace Persistify.Server.Commands.Documents;

public sealed class CreateDocumentCommand : Command<CreateDocumentRequest, CreateDocumentResponse>
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly ITemplateManager _templateManager;
    private Document? _document;

    public CreateDocumentCommand(
        ICommandContext<CreateDocumentRequest> commandContext,
        ITemplateManager templateManager,
        IDocumentManagerStore documentManagerStore
    ) : base(
        commandContext
    )
    {
        _templateManager = templateManager;
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(CreateDocumentRequest request, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(request.TemplateName);

        if (template is null)
        {
            throw new PersistifyInternalException();
        }

        _document = new Document
        {
            TextFieldValues = request.TextFieldValues,
            NumberFieldValues = request.NumberFieldValues,
            BoolFieldValues = request.BoolFieldValues
        };

        var documentManager = _documentManagerStore.GetManager(template.Id);

        if (documentManager is null)
        {
            throw new PersistifyInternalException();
        }

        await CommandContext.CurrentTransaction
            .PromoteManagerAsync(documentManager, true, TransactionTimeout);

        documentManager.Add(_document);
    }

    protected override CreateDocumentResponse GetResponse()
    {
        return new CreateDocumentResponse(_document?.Id ?? throw new PersistifyInternalException());
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
        return Permission.DocumentWrite;
    }
}
