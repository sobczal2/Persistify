using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.Internal;
using Persistify.Responses.Internal;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Users;
using Persistify.Server.Files;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Internal;

public class SetupFileSystemRequestHandler
    : RequestHandler<SetupFileSystemRequest, SetupFileSystemResponse>
{
    private readonly IFileHandler _fileHandler;

    public SetupFileSystemRequestHandler(
        IRequestHandlerContext<
            SetupFileSystemRequest,
            SetupFileSystemResponse
        > requestHandlerContext,
        IFileHandler fileHandler
    )
        : base(requestHandlerContext)
    {
        _fileHandler = fileHandler;
    }

    protected override async ValueTask RunAsync(
        SetupFileSystemRequest request,
        CancellationToken cancellationToken
    )
    {
        _fileHandler.EnsureRequiredFiles();

        await ValueTask.CompletedTask;
    }

    protected override SetupFileSystemResponse GetResponse()
    {
        return new SetupFileSystemResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(
        SetupFileSystemRequest request
    )
    {
        return new TransactionDescriptor(true, new List<IManager>(), new List<IManager>());
    }

    protected override Permission GetRequiredPermission(
        SetupFileSystemRequest request
    )
    {
        return Permission.Root;
    }
}
