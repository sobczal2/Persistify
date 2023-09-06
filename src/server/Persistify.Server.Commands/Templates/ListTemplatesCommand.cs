using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.Templates;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Commands.Common;
using Persistify.Server.Errors;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Commands.Templates;

public class ListTemplatesCommand : Command<ListTemplatesRequest, ListTemplatesResponse>
{
    private readonly ITemplateManager _templateManager;
    private List<Template>? _templates;
    private int _totalCount;

    public ListTemplatesCommand(
        IValidator<ListTemplatesRequest> validator,
        ILoggerFactory loggerFactory,
        ITemplateManager templateManager,
        ITransactionState transactionState
    ) : base(
        validator,
        loggerFactory,
        transactionState
    )
    {
        _templateManager = templateManager;
    }

    protected override async ValueTask RunAsync(ListTemplatesRequest data, CancellationToken cancellationToken)
    {
        var skip = data.Pagination.PageNumber * data.Pagination.PageSize;
        var take = data.Pagination.PageSize;
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

    protected override TransactionDescriptor GetTransactionDescriptor(ListTemplatesRequest data)
    {
        return new TransactionDescriptor(
            exclusiveGlobal: false,
            readManagers: new List<IManager> { _templateManager },
            writeManagers: new List<IManager>()
        );
    }
}
