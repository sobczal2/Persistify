using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Commands.Common;
using Persistify.Server.Errors;
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
        ITemplateManager templateManager
    ) : base(
        validator,
        loggerFactory
    )
    {
        _templateManager = templateManager;
    }

    protected override async ValueTask RunAsync(DeleteTemplateRequest data, CancellationToken cancellationToken)
    {
        var result = await _templateManager.RemoveAsync(data.TemplateId);

        if (!result)
        {
            throw new ValidationException(nameof(data.TemplateId), $"Template with id {data.TemplateId} not found");
        }
    }

    protected override DeleteTemplateResponse GetResponse()
    {
        return new DeleteTemplateResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(DeleteTemplateRequest data)
    {
        return new TransactionDescriptor(
            exclusiveGlobal: false,
            readManagers: new List<IManager>(),
            writeManagers: new List<IManager> { _templateManager }
        );
    }
}
