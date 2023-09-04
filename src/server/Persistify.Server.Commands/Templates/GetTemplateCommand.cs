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

public sealed class GetTemplateCommand : Command<GetTemplateRequest, GetTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private Template? _template;

    public GetTemplateCommand(
        IValidator<GetTemplateRequest> validator,
        ILoggerFactory loggerFactory,
        ITemplateManager templateManager
    ) : base(
        validator,
        loggerFactory
    )
    {
        _templateManager = templateManager;
    }

    protected override async ValueTask RunAsync(GetTemplateRequest data, CancellationToken cancellationToken)
    {
        _template = await _templateManager.GetAsync(data.TemplateId);

        if (_template is null)
        {
            throw new ValidationException(nameof(data.TemplateId), $"Template with id {data.TemplateId} not found");
        }
    }

    protected override GetTemplateResponse GetResponse()
    {
        if (_template is null)
        {
            throw new PersistifyInternalException();
        }

        return new GetTemplateResponse(_template);
    }

    protected override TransactionDescriptor GetTransactionDescriptor(GetTemplateRequest data)
    {
        return new TransactionDescriptor(
            exclusiveGlobal: false,
            readManagers: ImmutableList.Create<IManager>(_templateManager),
            writeManagers: ImmutableList<IManager>.Empty
        );
    }
}
