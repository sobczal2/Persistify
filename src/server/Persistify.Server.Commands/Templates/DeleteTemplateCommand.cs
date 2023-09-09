using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Commands.Templates;

public class DeleteTemplateCommand : Command<DeleteTemplateRequest, DeleteTemplateResponse>
{
    private readonly ITemplateManager _templateManager;

    public DeleteTemplateCommand(
        IValidator<DeleteTemplateRequest> validator,
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler,
        ITemplateManager templateManager
    ) : base(
        validator,
        loggerFactory,
        transactionState,
        exceptionHandler
    )
    {
        _templateManager = templateManager;
    }

    protected override async ValueTask RunAsync(DeleteTemplateRequest data, CancellationToken cancellationToken)
    {
        var template = await _templateManager.GetAsync(data.TemplateName);

        if (template is null)
        {
            throw new ValidationException(nameof(DeleteTemplateRequest.TemplateName), $"Template {data.TemplateName} not found");
        }

        var result = await _templateManager.RemoveAsync(template.Id);

        if (!result)
        {
            throw new PersistifyInternalException();
        }
    }

    protected override DeleteTemplateResponse GetResponse()
    {
        return new DeleteTemplateResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(DeleteTemplateRequest data)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _templateManager }
        );
    }
}
