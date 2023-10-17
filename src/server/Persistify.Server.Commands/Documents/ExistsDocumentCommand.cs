using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Documents;
using Persistify.Requests.Templates;
using Persistify.Responses.Documents;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Documents;

public class ExistsDocumentCommand : Command<ExistsDocumentRequest, ExistsDocumentResponse>
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly ITemplateManager _templateManager;
    private bool? _exists;

    public ExistsDocumentCommand(
        ICommandContext<ExistsDocumentRequest> commandContext,
        ITemplateManager templateManager,
        IDocumentManagerStore documentManagerStore
    ) : base(
        commandContext
    )
    {
        _templateManager = templateManager;
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(ExistsDocumentRequest request, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(request.TemplateName) ??
                       throw new InternalPersistifyException(nameof(DeleteDocumentRequest));

        var documentManager = _documentManagerStore.GetManager(template.Id) ??
                              throw new InternalPersistifyException(nameof(DeleteDocumentRequest));

        await CommandContext
            .CurrentTransaction
            .PromoteManagerAsync(documentManager, true, TransactionTimeout);

        _exists = await documentManager.ExistsAsync(request.DocumentId);
    }

    protected override ExistsDocumentResponse GetResponse()
    {
        return new ExistsDocumentResponse
        {
            Exists = _exists ?? throw new InternalPersistifyException(nameof(ExistsTemplateRequest))
        };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(ExistsDocumentRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _templateManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(ExistsDocumentRequest request)
    {
        return Permission.DocumentRead | Permission.TemplateRead;
    }
}
