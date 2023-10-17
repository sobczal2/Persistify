using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Internal;
using Persistify.Responses.Internal;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Internal;

public class InitializeUserManagerRequestHandler : RequestHandler<InitializeUserManagerRequest, InitializeUserManagerResponse>
{
    private readonly IUserManager _userManager;

    public InitializeUserManagerRequestHandler(
        IRequestHandlerContext<InitializeUserManagerRequest, InitializeUserManagerResponse> requestHandlerContext,
        IUserManager userManager
    ) : base(
        requestHandlerContext
    )
    {
        _userManager = userManager;
    }

    protected override ValueTask RunAsync(InitializeUserManagerRequest request, CancellationToken cancellationToken)
    {
        _userManager.Initialize();

        return ValueTask.CompletedTask;
    }

    protected override InitializeUserManagerResponse GetResponse()
    {
        return new InitializeUserManagerResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(InitializeUserManagerRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }

    protected override Permission GetRequiredPermission(InitializeUserManagerRequest request)
    {
        return Permission.UserWrite;
    }
}
