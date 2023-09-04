using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.Documents;
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

public sealed class CreateDocumentCommand : Command<CreateDocumentRequest, CreateDocumentResponse>
{
    private readonly ITemplateManager _templateManager;
    private readonly IDocumentManagerStore _documentManagerStore;
    private IDocumentManager? _documentManager;
    private Document? _document;

    public CreateDocumentCommand(
        IValidator<CreateDocumentRequest> validator,
        ILoggerFactory loggerFactory,
        ITemplateManager templateManager,
        IDocumentManagerStore documentManagerStore
    ) : base(
        validator,
        loggerFactory
    )
    {
        _templateManager = templateManager;
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(CreateDocumentRequest data, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(data.TemplateId);

        if (template is null)
        {
            // Edge case: template was deleted after start of transaction
            throw new ValidationException(nameof(data.TemplateId), $"Template with id {data.TemplateId} not found");
        }

        if (_documentManager is null)
        {
            throw new PersistifyInternalException();
        }

        _document = new Document
        {
            TextFieldValues = data.TextFieldValues,
            NumberFieldValues = data.NumberFieldValues,
            BoolFieldValues = data.BoolFieldValues
        };

        _documentManager.Add(_document);
    }

    protected override CreateDocumentResponse GetResponse()
    {
        if (_document is null)
        {
            throw new PersistifyInternalException();
        }

        return new CreateDocumentResponse(_document.Id);
    }

    protected override TransactionDescriptor GetTransactionDescriptor(CreateDocumentRequest data)
    {
        _documentManager = _documentManagerStore.GetManager(data.TemplateId);

        if (_documentManager is null)
        {
            throw new ValidationException(nameof(data.TemplateId), $"Template with id {data.TemplateId} not found");
        }

        return new TransactionDescriptor(
            exclusiveGlobal: false,
            readManagers: ImmutableList.Create<IManager>(_templateManager),
            writeManagers: ImmutableList.Create<IManager>(_documentManager!)
        );
    }
}
