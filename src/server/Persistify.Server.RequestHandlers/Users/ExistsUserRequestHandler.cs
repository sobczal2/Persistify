using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Users;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Users;

public class ExistsUserRequestHandler : RequestHandler<ExistsUserRequest, ExistsUserResponse>
{
    private readonly IUserManager _userManager;
    private bool? _exists;

    public ExistsUserRequestHandler(
        IRequestHandlerContext<ExistsUserRequest, ExistsUserResponse> requestHandlerContext,
        IUserManager userManager
    )
        : base(requestHandlerContext)
    {
        _userManager = userManager;
    }

    protected override ValueTask RunAsync(
        ExistsUserRequest request,
        CancellationToken cancellationToken
    )
    {
        _exists = _userManager.Exists(request.Username);

        return ValueTask.CompletedTask;
    }

    protected override ExistsUserResponse GetResponse()
    {
        return new ExistsUserResponse
        {
            Exists = _exists ?? throw new InternalPersistifyException(nameof(ExistsUserRequest))
        };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(ExistsUserRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _userManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(ExistsUserRequest request)
    {
        return Permission.UserRead;
    }
}
