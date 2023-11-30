using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.Internal;
using Persistify.Responses.Internal;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Users;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Internal;

public class InitializeTemplateManagerRequestHandler
    : RequestHandler<InitializeTemplateManagerRequest, InitializeTemplateManagerResponse>
{
    private readonly ITemplateManager _templateManager;

    public InitializeTemplateManagerRequestHandler(
        IRequestHandlerContext<
            InitializeTemplateManagerRequest,
            InitializeTemplateManagerResponse
        > requestHandlerContext,
        ITemplateManager templateManager
    )
        : base(requestHandlerContext)
    {
        _templateManager = templateManager;
    }

    protected override ValueTask RunAsync(
        InitializeTemplateManagerRequest request,
        CancellationToken cancellationToken
    )
    {
        _templateManager.Initialize();

        return ValueTask.CompletedTask;
    }

    protected override InitializeTemplateManagerResponse GetResponse()
    {
        return new InitializeTemplateManagerResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(
        InitializeTemplateManagerRequest request
    )
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _templateManager }
        );
    }

    protected override Permission GetRequiredPermission(
        InitializeTemplateManagerRequest request
    )
    {
        return Permission.TemplateWrite;
    }
}
