using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Domain.Users;
using Persistify.Dtos.Mappers;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Templates;

public sealed class GetTemplateRequestHandler : RequestHandler<GetTemplateRequest, GetTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private Template? _template;

    public GetTemplateRequestHandler(
        IRequestHandlerContext<GetTemplateRequest, GetTemplateResponse> requestHandlerContext,
        ITemplateManager templateManager
    ) : base(
        requestHandlerContext
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
        var template = _template ?? throw new InternalPersistifyException(nameof(GetTemplateRequest));
        return new GetTemplateResponse
        {
            Template = TemplateMapper.Map(template)
        };
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
