using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Domain.Users;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Templates;

public class ListTemplatesCommand : Command<ListTemplatesRequest, ListTemplatesResponse>
{
    private readonly ITemplateManager _templateManager;
    private List<Template>? _templates;
    private int? _totalCount;

    public ListTemplatesCommand(
        ICommandContext<ListTemplatesRequest> commandContext,
        ITemplateManager templateManager
    ) : base(
        commandContext
    )
    {
        _templateManager = templateManager;
    }

    protected override async ValueTask RunAsync(ListTemplatesRequest request, CancellationToken cancellationToken)
    {
        var skip = request.Pagination.PageNumber * request.Pagination.PageSize;
        var take = request.Pagination.PageSize;
        _templates = await _templateManager.ListAsync(take, skip).ToListAsync(cancellationToken);
        _totalCount = _templateManager.Count();
    }

    protected override ListTemplatesResponse GetResponse()
    {
        return new ListTemplatesResponse
        {
            Templates = _templates ?? throw new InternalPersistifyException(nameof(ListTemplatesRequest)),
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
