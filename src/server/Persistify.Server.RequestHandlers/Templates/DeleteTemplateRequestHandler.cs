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

public class DeleteTemplateRequestHandler
    : RequestHandler<DeleteTemplateRequest, DeleteTemplateResponse>
{
    private readonly ITemplateManager _templateManager;

    public DeleteTemplateRequestHandler(
        IRequestHandlerContext<DeleteTemplateRequest, DeleteTemplateResponse> requestHandlerContext,
        ITemplateManager templateManager
    )
        : base(requestHandlerContext)
    {
        _templateManager = templateManager;
    }

    protected override async ValueTask RunAsync(
        DeleteTemplateRequest request,
        CancellationToken cancellationToken
    )
    {
        var template = await _templateManager.GetAsync(request.TemplateName);

        if (template is null)
        {
            throw new InternalPersistifyException(nameof(DeleteTemplateRequest));
        }

        var result = await _templateManager.RemoveAsync(template.Id);

        if (!result)
        {
            throw new InternalPersistifyException(nameof(DeleteTemplateRequest));
        }
    }

    protected override DeleteTemplateResponse GetResponse()
    {
        return new DeleteTemplateResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(DeleteTemplateRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _templateManager }
        );
    }

    protected override Permission GetRequiredPermission(DeleteTemplateRequest request)
    {
        return Permission.TemplateWrite;
    }
}
