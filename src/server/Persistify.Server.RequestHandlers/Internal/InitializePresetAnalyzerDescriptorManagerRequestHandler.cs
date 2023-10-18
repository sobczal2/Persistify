using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Internal;
using Persistify.Responses.Internal;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Fts.Presets;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.PresetAnalyzerDescriptors;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Internal;

public class InitializePresetAnalyzerDescriptorManagerRequestHandler : RequestHandler<
    InitializePresetAnalyzerDescriptorManagerRequest, InitializePresetAnalyzerDescriptorManagerResponse>
{
    private readonly IPresetAnalyzerDescriptorManager _presetAnalyzerDescriptorManager;
    private readonly IEnumerable<IBuiltInPresetAnalyzerDescriptor> _standardPresetAnalyzerDescriptors;

    public InitializePresetAnalyzerDescriptorManagerRequestHandler(
        IRequestHandlerContext<InitializePresetAnalyzerDescriptorManagerRequest,
            InitializePresetAnalyzerDescriptorManagerResponse> requestHandlerContext,
        IPresetAnalyzerDescriptorManager presetAnalyzerDescriptorManager,
        IEnumerable<IBuiltInPresetAnalyzerDescriptor> standardPresetAnalyzerDescriptors
            ) : base(
        requestHandlerContext
    )
    {
        _presetAnalyzerDescriptorManager = presetAnalyzerDescriptorManager;
        _standardPresetAnalyzerDescriptors = standardPresetAnalyzerDescriptors;
    }

    protected override ValueTask RunAsync(InitializePresetAnalyzerDescriptorManagerRequest request,
        CancellationToken cancellationToken)
    {
        _presetAnalyzerDescriptorManager.Initialize();

        foreach (var standardPresetAnalyzerDescriptor in _standardPresetAnalyzerDescriptors)
        {
            var presetAnalyzerDescriptor = standardPresetAnalyzerDescriptor.GetDescriptor();
            if (!_presetAnalyzerDescriptorManager.Exists(presetAnalyzerDescriptor.Name))
            {
                _presetAnalyzerDescriptorManager.Add(presetAnalyzerDescriptor);
            }
        }

        return ValueTask.CompletedTask;
    }

    protected override InitializePresetAnalyzerDescriptorManagerResponse GetResponse()
    {
        return new InitializePresetAnalyzerDescriptorManagerResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(
        InitializePresetAnalyzerDescriptorManagerRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _presetAnalyzerDescriptorManager }
        );
    }

    protected override Permission GetRequiredPermission(InitializePresetAnalyzerDescriptorManagerRequest request)
    {
        return Permission.PresetAnalyzerDescriptorWrite;
    }
}
