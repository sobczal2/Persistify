using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Internal;
using Persistify.Responses.Internal;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.PresetAnalyzers;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Internal;

public class InitializePresetAnalyzerManagerRequestHandler : RequestHandler<
    InitializePresetAnalyzerManagerRequest, InitializePresetAnalyzerManagerResponse>
{
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;

    public InitializePresetAnalyzerManagerRequestHandler(
        IRequestHandlerContext<InitializePresetAnalyzerManagerRequest,
            InitializePresetAnalyzerManagerResponse> requestHandlerContext,
        IPresetAnalyzerManager presetAnalyzerManager
            ) : base(
        requestHandlerContext
    )
    {
        _presetAnalyzerManager = presetAnalyzerManager;
    }

    protected override ValueTask RunAsync(InitializePresetAnalyzerManagerRequest request,
        CancellationToken cancellationToken)
    {
        _presetAnalyzerManager.Initialize();

        return ValueTask.CompletedTask;
    }

    protected override InitializePresetAnalyzerManagerResponse GetResponse()
    {
        return new InitializePresetAnalyzerManagerResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(
        InitializePresetAnalyzerManagerRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _presetAnalyzerManager }
        );
    }

    protected override Permission GetRequiredPermission(InitializePresetAnalyzerManagerRequest request)
    {
        return Permission.PresetAnalyzerWrite;
    }
}
