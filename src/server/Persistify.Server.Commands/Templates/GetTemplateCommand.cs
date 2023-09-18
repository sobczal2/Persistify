using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Domain.Users;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Templates;

public sealed class GetTemplateCommand : Command<GetTemplateRequest, GetTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private Template? _template;

    public GetTemplateCommand(
        ICommandContext<GetTemplateRequest> commandContext,
        ITemplateManager templateManager
    ) : base(
        commandContext
    )
    {
        _templateManager = templateManager;
    }

    protected override async ValueTask RunAsync(GetTemplateRequest request, CancellationToken cancellationToken)
    {
        _template = await _templateManager.GetAsync(request.TemplateName);
    }

    protected override GetTemplateResponse GetResponse()
    {
        return new GetTemplateResponse(_template ?? throw new PersistifyInternalException());
    }

    protected override TransactionDescriptor GetTransactionDescriptor(GetTemplateRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _templateManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(GetTemplateRequest request)
    {
        return Permission.TemplateRead;
    }
}
