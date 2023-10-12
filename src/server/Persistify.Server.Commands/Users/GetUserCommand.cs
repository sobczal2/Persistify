using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Users;

public class GetUserCommand : Command<GetUserRequest, GetUserResponse>
{
    private readonly IUserManager _userManager;
    private User? _user;

    public GetUserCommand(
        ICommandContext<GetUserRequest> commandContext,
        IUserManager userManager
    ) : base(
        commandContext
    )
    {
        _userManager = userManager;
    }

    protected override async ValueTask RunAsync(GetUserRequest request, CancellationToken cancellationToken)
    {
        _user = await _userManager.GetAsync(request.Username) ?? throw new InternalPersistifyException(nameof(GetUserRequest));
    }

    protected override GetUserResponse GetResponse()
    {
        if (_user is null)
        {
            throw new InternalPersistifyException(nameof(GetUserRequest));
        }

        return new GetUserResponse { Username = _user.Username, Permission = (int)_user.Permission };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(GetUserRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _userManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(GetUserRequest request)
    {
        return Permission.UserRead;
    }
}
