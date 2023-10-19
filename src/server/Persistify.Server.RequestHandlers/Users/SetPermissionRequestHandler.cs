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

public class SetPermissionRequestHandler : RequestHandler<SetPermissionRequest, SetPermissionResponse>
{
    private readonly IUserManager _userManager;

    public SetPermissionRequestHandler(
        IRequestHandlerContext<SetPermissionRequest, SetPermissionResponse> requestHandlerContext,
        IUserManager userManager
    ) : base(
        requestHandlerContext
    )
    {
        _userManager = userManager;
    }

    protected override async ValueTask RunAsync(SetPermissionRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetAsync(request.Username) ??
                   throw new InternalPersistifyException(nameof(SetPermissionRequest));

        user.Permission = (Permission)request.Permission;

        await _userManager.Update(user);
    }

    protected override SetPermissionResponse GetResponse()
    {
        return new SetPermissionResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(SetPermissionRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager });
    }

    protected override Permission GetRequiredPermission(SetPermissionRequest request)
    {
        return Permission.UserWrite;
    }
}
