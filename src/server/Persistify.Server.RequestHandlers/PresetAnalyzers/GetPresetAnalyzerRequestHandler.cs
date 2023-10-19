using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.PresetAnalyzers;
using Persistify.Domain.Users;
using Persistify.Dtos.Mappers;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Responses.PresetAnalyzers;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.PresetAnalyzers;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.PresetAnalyzers;

public class GetPresetAnalyzerRequestHandler : RequestHandler<GetPresetAnalyzerRequest, GetPresetAnalyzerResponse>
{
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;
    private PresetAnalyzer? _presetAnalyzer;

    public GetPresetAnalyzerRequestHandler(
        IRequestHandlerContext<GetPresetAnalyzerRequest, GetPresetAnalyzerResponse> requestHandlerContext,
        IPresetAnalyzerManager presetAnalyzerManager
        ) : base(
        requestHandlerContext
        )
    {
        _presetAnalyzerManager = presetAnalyzerManager;
    }

    protected override async ValueTask RunAsync(GetPresetAnalyzerRequest request, CancellationToken cancellationToken)
    {
        _presetAnalyzer = await _presetAnalyzerManager.GetAsync(request.PresetAnalyzerName);
    }

    protected override GetPresetAnalyzerResponse GetResponse()
    {
        var presetAnalyzer = _presetAnalyzer ?? throw new InternalPersistifyException(nameof(GetPresetAnalyzerRequest));
        return new GetPresetAnalyzerResponse
        {
            PresetAnalyzerDto = PresetAnalyzerMapper.Map(presetAnalyzer)
        };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(GetPresetAnalyzerRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _presetAnalyzerManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(GetPresetAnalyzerRequest request)
    {
        return Permission.PresetAnalyzerRead;
    }
}
