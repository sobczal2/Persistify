using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Templates;

public class ExistsTemplateCommand : Command<ExistsTemplateRequest, ExistsTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private bool? _exists;

    public ExistsTemplateCommand(
        ICommandContext<ExistsTemplateRequest> commandContext,
        ITemplateManager templateManager
    ) : base(
        commandContext
    )
    {
        _templateManager = templateManager;
    }

    protected override ValueTask RunAsync(ExistsTemplateRequest request, CancellationToken cancellationToken)
    {
        _exists = _templateManager.Exists(request.TemplateName);

        return ValueTask.CompletedTask;
    }

    protected override ExistsTemplateResponse GetResponse()
    {
        return new ExistsTemplateResponse
        {
            Exists = _exists ?? throw new InternalPersistifyException(nameof(ExistsTemplateRequest))
        };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(ExistsTemplateRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _templateManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(ExistsTemplateRequest request)
    {
        return Permission.TemplateRead;
    }
}
