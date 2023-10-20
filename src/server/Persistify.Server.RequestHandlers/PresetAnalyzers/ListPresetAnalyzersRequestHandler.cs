using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Helpers.Collections;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Responses.PresetAnalyzers;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.PresetAnalyzers;
using Persistify.Server.Domain.Users;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.PresetAnalyzers;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Mappers.PresetAnalyzers;

namespace Persistify.Server.CommandHandlers.PresetAnalyzers;

public class ListPresetAnalyzersRequestHandler : RequestHandler<ListPresetAnalyzersRequest, ListPresetAnalyzersResponse>
{
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;
    private List<PresetAnalyzer>? _presetAnalyzers;
    private int? _totalCount;

    public ListPresetAnalyzersRequestHandler(
        IRequestHandlerContext<ListPresetAnalyzersRequest, ListPresetAnalyzersResponse> requestHandlerContext,
        IPresetAnalyzerManager presetAnalyzerManager
    ) : base(
        requestHandlerContext
    )
    {
        _presetAnalyzerManager = presetAnalyzerManager;
    }

    protected override async ValueTask RunAsync(ListPresetAnalyzersRequest request, CancellationToken cancellationToken)
    {
        var skip = request.PaginationDto.PageNumber * request.PaginationDto.PageSize;
        var take = request.PaginationDto.PageSize;

        _presetAnalyzers = await _presetAnalyzerManager.ListAsync(take, skip).ToListAsync(cancellationToken);
        _totalCount = _presetAnalyzerManager.Count();
    }

    protected override ListPresetAnalyzersResponse GetResponse()
    {
        var presetAnalyzers =
            _presetAnalyzers ?? throw new InternalPersistifyException(nameof(ListPresetAnalyzersRequest));
        return new ListPresetAnalyzersResponse
        {
            PresetAnalyzerDtos = presetAnalyzers.ListSelect(x => x.ToDto()),
            TotalCount = _totalCount ?? throw new InternalPersistifyException(nameof(ListPresetAnalyzersRequest))
        };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(ListPresetAnalyzersRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _presetAnalyzerManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(ListPresetAnalyzersRequest request)
    {
        return Permission.PresetAnalyzerRead;
    }
}
