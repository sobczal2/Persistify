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

public class DeleteUserRequestHandler : RequestHandler<DeleteUserRequest, DeleteUserResponse>
{
    private readonly IUserManager _userManager;

    public DeleteUserRequestHandler(
        IRequestHandlerContext<DeleteUserRequest, DeleteUserResponse> requestHandlerContext,
        IUserManager userManager
    ) : base(
        requestHandlerContext
    )
    {
        _userManager = userManager;
    }

    protected override async ValueTask RunAsync(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetAsync(request.Username) ??
                   throw new InternalPersistifyException(nameof(DeleteUserRequest));

        if (!await _userManager.RemoveAsync(user.Id))
        {
            throw new InternalPersistifyException(nameof(DeleteUserRequest));
        }
    }

    protected override DeleteUserResponse GetResponse()
    {
        return new DeleteUserResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(DeleteUserRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }

    protected override Permission GetRequiredPermission(DeleteUserRequest request)
    {
        return Permission.UserWrite;
    }
}
