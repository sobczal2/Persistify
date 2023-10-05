using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Shared;
using Persistify.Server.Commands.Common;
using Persistify.Server.Files;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Internal.Management;

public class SetupFileSystemCommand : Command
{
    private readonly IFileHandler _fileHandler;

    public SetupFileSystemCommand(
        ICommandContext commandContext,
        IFileHandler fileHandler
    ) : base(
        commandContext
    )
    {
        _fileHandler = fileHandler;
    }

    protected override async ValueTask RunAsync(EmptyRequest request, CancellationToken cancellationToken)
    {
        _fileHandler.EnsureRequiredFiles();

        await ValueTask.CompletedTask;
    }

    protected override TransactionDescriptor GetTransactionDescriptor(EmptyRequest request)
    {
        return new TransactionDescriptor(
            true,
            new List<IManager>(),
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(EmptyRequest request)
    {
        return Permission.Root;
    }
}
