using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Responses.PresetAnalyzers;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Users;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.PresetAnalyzers;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.PresetAnalyzers;

public class ExistsPresetAnalyzerRequestHandler
    : RequestHandler<ExistsPresetAnalyzerRequest, ExistsPresetAnalyzerResponse>
{
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;
    private bool? _exists;

    public ExistsPresetAnalyzerRequestHandler(
        IRequestHandlerContext<
            ExistsPresetAnalyzerRequest,
            ExistsPresetAnalyzerResponse
        > requestHandlerContext,
        IPresetAnalyzerManager presetAnalyzerManager
    )
        : base(requestHandlerContext)
    {
        _presetAnalyzerManager = presetAnalyzerManager;
    }

    protected override ValueTask RunAsync(
        ExistsPresetAnalyzerRequest request,
        CancellationToken cancellationToken
    )
    {
        _exists = _presetAnalyzerManager.Exists(request.PresetAnalyzerName);

        return ValueTask.CompletedTask;
    }

    protected override ExistsPresetAnalyzerResponse GetResponse()
    {
        return new ExistsPresetAnalyzerResponse
        {
            Exists =
                _exists
                ?? throw new InternalPersistifyException(nameof(ExistsPresetAnalyzerRequest))
        };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(
        ExistsPresetAnalyzerRequest request
    )
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _presetAnalyzerManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(ExistsPresetAnalyzerRequest request)
    {
        return Permission.PresetAnalyzerRead;
    }
}
