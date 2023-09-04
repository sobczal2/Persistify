﻿using System.Collections.Generic;
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
        ITemplateManager templateManager
    ) : base(
        validator,
        loggerFactory
    )
    {
        _templateManager = templateManager;
    }

    protected override async ValueTask RunAsync(ListTemplatesRequest data, CancellationToken cancellationToken)
    {
        var skip = data.Pagination.PageNumber * data.Pagination.PageSize;
        var take = data.Pagination.PageSize;
        _templates = await _templateManager.ListAsync(take, skip);
        _totalCount = await _templateManager.CountAsync();
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
            readManagers: ImmutableList.Create<IManager>(_templateManager),
            writeManagers: ImmutableList<IManager>.Empty
        );
    }
}
