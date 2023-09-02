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

        TransactionDescriptor = new TransactionDescriptor(
            exclusiveGlobal: false,
            readManagers: ImmutableList<IManager>.Empty,
            writeManagers: ImmutableList.Create<IManager>(_templateManager)
            );
    }

    protected override async ValueTask ExecuteAsync(DeleteTemplateRequest data, CancellationToken cancellationToken)
    {
        var result = await _templateManager.RemoveAsync(data.TemplateId);

        if (!result)
        {
            new ValidationException(nameof(data.TemplateId), $"Template with id {data.TemplateId} not found").Throw();
        }
    }

    protected override DeleteTemplateResponse GetResponse()
    {
        return new DeleteTemplateResponse();
    }

    protected override TransactionDescriptor TransactionDescriptor { get; }
}
