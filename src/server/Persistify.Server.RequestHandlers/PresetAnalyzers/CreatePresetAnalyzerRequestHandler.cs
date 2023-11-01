using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Responses.PresetAnalyzers;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.PresetAnalyzers;
using Persistify.Server.Domain.Users;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.PresetAnalyzers;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Mappers.PresetAnalyzers;

namespace Persistify.Server.CommandHandlers.PresetAnalyzers;

public class CreatePresetAnalyzerRequestHandler
    : RequestHandler<CreatePresetAnalyzerRequest, CreatePresetAnalyzerResponse>
{
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;

    public CreatePresetAnalyzerRequestHandler(
        IRequestHandlerContext<
            CreatePresetAnalyzerRequest,
            CreatePresetAnalyzerResponse
        > requestHandlerContext,
        IPresetAnalyzerManager presetAnalyzerManager
    )
        : base(requestHandlerContext)
    {
        _presetAnalyzerManager = presetAnalyzerManager;
    }

    protected override ValueTask RunAsync(
        CreatePresetAnalyzerRequest request,
        CancellationToken cancellationToken
    )
    {
        var presetAnalyzer = new PresetAnalyzer
        {
            Name = request.PresetAnalyzerName, Analyzer = request.FullAnalyzerDto.ToDomain()
        };

        _presetAnalyzerManager.Add(presetAnalyzer);

        return ValueTask.CompletedTask;
    }

    protected override CreatePresetAnalyzerResponse GetResponse()
    {
        return new CreatePresetAnalyzerResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(
        CreatePresetAnalyzerRequest request
    )
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _presetAnalyzerManager }
        );
    }

    protected override Permission GetRequiredPermission(CreatePresetAnalyzerRequest request)
    {
        return Permission.PresetAnalyzerWrite;
    }
}
