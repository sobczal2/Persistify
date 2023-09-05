﻿using System.Collections.Generic;
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
            throw new ValidationException(nameof(data.TemplateId), $"Template with id {data.TemplateId} not found");
        }

        var documentManager = _documentManagerStore.GetManager(template.Id);

        if (documentManager is null)
        {
            throw new PersistifyInternalException();
        }

        await Transaction.GetCurrentTransaction().PromoteManagerAsync(documentManager, true, TransactionTimeout);

        _document = new Document
        {
            TextFieldValues = data.TextFieldValues,
            NumberFieldValues = data.NumberFieldValues,
            BoolFieldValues = data.BoolFieldValues
        };

        documentManager.Add(_document);
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
        return new TransactionDescriptor(
            exclusiveGlobal: false,
            readManagers: new List<IManager> { _templateManager },
            writeManagers: new List<IManager>()
        );
    }
}
