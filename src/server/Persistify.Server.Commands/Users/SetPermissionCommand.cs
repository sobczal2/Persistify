using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using Persistify.Server.Commands.Common;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Users;

namespace Persistify.Server.Commands.Users;

public class SetPermissionCommand : Command<SetPermissionRequest, SetPermissionResponse>
{
    private readonly IUserManager _userManager;

    public SetPermissionCommand(
        ICommandContext<SetPermissionRequest> commandContext,
        IUserManager userManager
    ) : base(
        commandContext
    )
    {
        _userManager = userManager;
    }

    protected override async ValueTask RunAsync(SetPermissionRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetAsync(request.Username);

        if (user is null)
        {
            throw new ValidationException(nameof(GetUserRequest.Username), UserErrorMessages.UserNotFound);
        }

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
