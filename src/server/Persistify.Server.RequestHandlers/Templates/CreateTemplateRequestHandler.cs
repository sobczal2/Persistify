﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Domain.Users;
using Persistify.Dtos.Mappers;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.CommandHandlers.Templates;

public sealed class CreateTemplateRequestHandler : RequestHandler<CreateTemplateRequest, CreateTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private Template? _template;

    public CreateTemplateRequestHandler(
        IRequestHandlerContext<CreateTemplateRequest, CreateTemplateResponse> requestHandlerContext,
        ITemplateManager templateManager
    ) : base(
        requestHandlerContext
    )
    {
        _templateManager = templateManager;
    }

    protected override ValueTask RunAsync(CreateTemplateRequest request, CancellationToken cancellationToken)
    {
        _template = new Template
        {
            Name = request.TemplateName,
            Fields = request.Fields.Select(FieldMapper.Map).ToList()
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
