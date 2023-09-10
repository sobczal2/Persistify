using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Shared;
using Persistify.Responses.Shared;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Files;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Internal.Management;

public class SetupFileSystemCommand : Command
{
    private readonly IFileHandler _fileHandler;

    public SetupFileSystemCommand(
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler,
        IFileHandler fileHandler
    ) : base(
        loggerFactory,
        transactionState,
        exceptionHandler
    )
    {
        _fileHandler = fileHandler;
    }

    protected override async ValueTask RunAsync(EmptyRequest data, CancellationToken cancellationToken)
    {
        _fileHandler.EnsureRequiredFiles();

        await ValueTask.CompletedTask;
    }

    protected override EmptyResponse GetResponse()
    {
        return new EmptyResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(EmptyRequest data)
    {
        return new TransactionDescriptor(
            true,
            new List<IManager>(),
            new List<IManager>()
        );
    }
}
