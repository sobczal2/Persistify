using System.Collections.Generic;
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

public sealed class CreateTemplateCommand : Command<CreateTemplateRequest, CreateTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private Template? _template;

    public CreateTemplateCommand(
        ICommandContext<CreateTemplateRequest> commandContext,
        ITemplateManager templateManager
    ) : base(
        commandContext
    )
    {
        _templateManager = templateManager;
    }

    protected override ValueTask RunAsync(CreateTemplateRequest request, CancellationToken cancellationToken)
    {
        _template = new Template
        {
            Name = request.TemplateName,
            TextFields = request.TextFields,
            NumberFields = request.NumberFields,
            BoolFields = request.BoolFields
        };

        _templateManager.Add(_template);

        return ValueTask.CompletedTask;
    }

    protected override CreateTemplateResponse GetResponse()
    {
        if (_template is null)
        {
            throw new InternalPersistifyException(nameof(CreateTemplateResponse));
        }

        return new CreateTemplateResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(CreateTemplateRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _templateManager }
        );
    }

    protected override Permission GetRequiredPermission(CreateTemplateRequest request)
    {
        return Permission.TemplateWrite;
    }
}
