using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Dtos.Templates.Common;
using Persistify.Dtos.Templates.Fields;
using Persistify.Helpers.Collections;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Templates;
using Persistify.Server.Domain.Users;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Mappers.Templates;

namespace Persistify.Server.CommandHandlers.Templates;

public class ListTemplatesRequestHandler : RequestHandler<ListTemplatesRequest, ListTemplatesResponse>
{
    private readonly ITemplateManager _templateManager;
    private List<Template>? _templates;
    private int? _totalCount;

    public ListTemplatesRequestHandler(
        IRequestHandlerContext<ListTemplatesRequest, ListTemplatesResponse> requestHandlerContext,
        ITemplateManager templateManager
    ) : base(
        requestHandlerContext
    )
    {
        _templateManager = templateManager;
    }

    protected override async ValueTask RunAsync(ListTemplatesRequest request, CancellationToken cancellationToken)
    {
        var skip = request.PaginationDto.PageNumber * request.PaginationDto.PageSize;
        var take = request.PaginationDto.PageSize;
        _templates = await _templateManager.ListAsync(take, skip).ToListAsync(cancellationToken);
        _totalCount = _templateManager.Count();
    }

    protected override ListTemplatesResponse GetResponse()
    {
        var templates = _templates ?? throw new InternalPersistifyException(nameof(ListTemplatesRequest));
        return new ListTemplatesResponse
        {
            TemplateDtos = templates.ListSelect(x => x.ToDto()),
            TotalCount = _totalCount ?? throw new InternalPersistifyException(nameof(ListTemplatesRequest))
        };
    }

    protected override TransactionDescriptor GetTransactionDescriptor(ListTemplatesRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _templateManager },
            new List<IManager>()
        );
    }

    protected override Permission GetRequiredPermission(ListTemplatesRequest request)
    {
        return Permission.TemplateRead;
    }
}
