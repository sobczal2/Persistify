using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Users;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Templates;

public class ExistsTemplateRequestHandler : RequestHandler<ExistsTemplateRequest, ExistsTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private bool? _exists;

    public ExistsTemplateRequestHandler(
        IRequestHandlerContext<ExistsTemplateRequest, ExistsTemplateResponse> requestHandlerContext,
        ITemplateManager templateManager
    ) : base(
        requestHandlerContext
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
