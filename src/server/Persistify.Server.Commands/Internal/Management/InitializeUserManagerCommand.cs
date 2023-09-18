using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Shared;
using Persistify.Server.Commands.Common;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Internal.Management;

public class InitializeUserManagerCommand : Command
{
    private readonly IUserManager _userManager;

    public InitializeUserManagerCommand(
        ICommandContext commandContext,
        IUserManager userManager
    ) : base(
        commandContext
    )
    {
        _userManager = userManager;
    }

    protected override ValueTask RunAsync(EmptyRequest request, CancellationToken cancellationToken)
    {
        _userManager.Initialize();

        return ValueTask.CompletedTask;
    }

    protected override TransactionDescriptor GetTransactionDescriptor(EmptyRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _userManager }
        );
    }

    protected override Permission GetRequiredPermission(EmptyRequest request)
    {
        return Permission.UserWrite;
    }
}
