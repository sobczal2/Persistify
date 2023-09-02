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

public sealed class CreateTemplateCommand : Command<CreateTemplateRequest, CreateTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private Template? _template;
    public CreateTemplateCommand(
        IValidator<CreateTemplateRequest> validator,
        ILoggerFactory loggerFactory,
        ITemplateManager templateManager
    ) : base(
        validator,
        loggerFactory
    )
    {
        _templateManager = templateManager;

        TransactionDescriptor = new TransactionDescriptor(
            exclusiveGlobal: false,
            readManagers: ImmutableList<IManager>.Empty,
            writeManagers: ImmutableList.Create<IManager>(_templateManager)
        );
    }

    protected override ValueTask ExecuteAsync(CreateTemplateRequest data, CancellationToken cancellationToken)
    {
        _template = new Template
        {
            Name = data.TemplateName,
            TextFields = data.TextFields,
            NumberFields = data.NumberFields,
            BoolFields = data.BoolFields
        };

        _templateManager.Add(_template);

        return ValueTask.CompletedTask;
    }

    protected override CreateTemplateResponse GetResponse()
    {
        if (_template is null)
        {
            throw new PersistifyInternalException();
        }

        return new CreateTemplateResponse(_template.Id);
    }

    protected override TransactionDescriptor TransactionDescriptor { get; }
}
