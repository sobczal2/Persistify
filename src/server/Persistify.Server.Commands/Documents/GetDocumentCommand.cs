﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.Documents;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Commands.Documents;

public class GetDocumentCommand : Command<GetDocumentRequest, GetDocumentResponse>
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly ITemplateManager _templateManager;
    private Document? _document;

    public GetDocumentCommand(
        IValidator<GetDocumentRequest> validator,
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler,
        ITemplateManager templateManager,
        IDocumentManagerStore documentManagerStore
    ) : base(
        validator,
        loggerFactory,
        transactionState,
        exceptionHandler
    )
    {
        _templateManager = templateManager;
        _documentManagerStore = documentManagerStore;
    }

    protected override async ValueTask RunAsync(GetDocumentRequest data, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(data.TemplateName);

        if (template is null)
        {
            throw new ValidationException(nameof(GetDocumentRequest.TemplateName), $"Template {data.TemplateName} not found");
        }

        var documentManager = _documentManagerStore.GetManager(template.Id);

        if (documentManager is null)
        {
            throw new PersistifyInternalException();
        }

        await TransactionState.GetCurrentTransaction().PromoteManagerAsync(documentManager, true, TransactionTimeout);

        _document = await documentManager.GetAsync(data.DocumentId);

        if (_document is null)
        {
            throw new ValidationException(nameof(GetDocumentRequest.DocumentId), $"Document with id {data.DocumentId} not found");
        }
    }

    protected override GetDocumentResponse GetResponse()
    {
        if (_document is null)
        {
            throw new PersistifyInternalException();
        }

        return new GetDocumentResponse(_document);
    }

    protected override TransactionDescriptor GetTransactionDescriptor(GetDocumentRequest data)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _templateManager },
            new List<IManager>()
        );
    }
}
