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

namespace Persistify.Server.Commands.Users;

public class DeleteUserCommand : Command<DeleteUserRequest, DeleteUserResponse>
{
    private readonly IUserManager _userManager;

    public DeleteUserCommand(
        ICommandContext<DeleteUserRequest> commandContext,
        IUserManager userManager
    ) : base(
        commandContext
    )
    {
        _userManager = userManager;
    }

    protected override ValueTask RunAsync(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    protected override DeleteUserResponse GetResponse()
    {
        throw new System.NotImplementedException();
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
