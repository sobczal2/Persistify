using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.Internal;
using Persistify.Responses.Internal;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Users;
using Persistify.Server.Fts.PresetAnalyzers;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.PresetAnalyzers;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Internal;

public class EnsureBuildInPresetAnalyzersExistRequestHandler : RequestHandler<EnsureBuildInPresetAnalyzersExistRequest,
    EnsureBuildInPresetAnalyzersExistResponse>
{
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;
    private readonly IEnumerable<IBuiltInPresetAnalyzer> _buildInPresetAnalyzers;

    public EnsureBuildInPresetAnalyzersExistRequestHandler(
        IRequestHandlerContext<EnsureBuildInPresetAnalyzersExistRequest, EnsureBuildInPresetAnalyzersExistResponse>
            requestHandlerContext,
        IPresetAnalyzerManager presetAnalyzerManager,
        IEnumerable<IBuiltInPresetAnalyzer> buildInPresetAnalyzers
    ) : base(
        requestHandlerContext
    )
    {
        _presetAnalyzerManager = presetAnalyzerManager;
        _buildInPresetAnalyzers = buildInPresetAnalyzers;
    }

    protected override ValueTask RunAsync(EnsureBuildInPresetAnalyzersExistRequest request,
        CancellationToken cancellationToken)
    {
        foreach (var builtInPresetAnalyzer in _buildInPresetAnalyzers)
        {
            var presetAnalyzer = builtInPresetAnalyzer.GetPresetAnalyzer();
            if (!_presetAnalyzerManager.Exists(presetAnalyzer.Name))
            {
                _presetAnalyzerManager.Add(presetAnalyzer);
            }
        }

        return ValueTask.CompletedTask;
    }

    protected override EnsureBuildInPresetAnalyzersExistResponse GetResponse()
    {
        return new EnsureBuildInPresetAnalyzersExistResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(EnsureBuildInPresetAnalyzersExistRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _presetAnalyzerManager }
        );
    }

    protected override Permission GetRequiredPermission(EnsureBuildInPresetAnalyzersExistRequest request)
    {
        return Permission.PresetAnalyzerWrite;
    }
}
