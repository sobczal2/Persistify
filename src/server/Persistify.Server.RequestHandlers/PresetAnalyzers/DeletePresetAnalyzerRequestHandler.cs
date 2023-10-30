using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Responses.PresetAnalyzers;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Users;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.PresetAnalyzers;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.PresetAnalyzers;

public class DeletePresetAnalyzerRequestHandler
    : RequestHandler<DeletePresetAnalyzerRequest, DeletePresetAnalyzerResponse>
{
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;

    public DeletePresetAnalyzerRequestHandler(
        IRequestHandlerContext<
            DeletePresetAnalyzerRequest,
            DeletePresetAnalyzerResponse
        > requestHandlerContext,
        IPresetAnalyzerManager presetAnalyzerManager
    )
        : base(requestHandlerContext)
    {
        _presetAnalyzerManager = presetAnalyzerManager;
    }

    protected override ValueTask RunAsync(
        DeletePresetAnalyzerRequest request,
        CancellationToken cancellationToken
    )
    {
        _presetAnalyzerManager.RemoveAsync(request.PresetAnalyzerName);

        return ValueTask.CompletedTask;
    }

    protected override DeletePresetAnalyzerResponse GetResponse()
    {
        return new DeletePresetAnalyzerResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(
        DeletePresetAnalyzerRequest request
    )
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _presetAnalyzerManager }
        );
    }

    protected override Permission GetRequiredPermission(DeletePresetAnalyzerRequest request)
    {
        return Permission.PresetAnalyzerWrite;
    }
}
