using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Domain.Users;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Templates;

public class ListTemplatesCommand : Command<ListTemplatesRequest, ListTemplatesResponse>
{
    private readonly ITemplateManager _templateManager;
    private List<Template>? _templates;
    private int _totalCount;

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
        _templates = await _templateManager.ListAsync(take, skip);
        _totalCount = _templateManager.Count();
    }

    protected override ListTemplatesResponse GetResponse()
    {
        if (_templates is null)
        {
            throw new PersistifyInternalException();
        }

        return new ListTemplatesResponse(_templates, _totalCount);
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
