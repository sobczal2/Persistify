﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Shared;
using Persistify.Responses.Shared;
using Persistify.Server.Commands.Common;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Commands.Internal.Management;

public class InitializeTemplateManagerCommand : Command
{
    private readonly ITemplateManager _templateManager;

    public InitializeTemplateManagerCommand(
        ILoggerFactory loggerFactory,
        ITemplateManager templateManager,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler
    ) : base(
        loggerFactory,
        transactionState,
        exceptionHandler
    )
    {
        _templateManager = templateManager;
    }

    protected override ValueTask RunAsync(EmptyRequest data, CancellationToken cancellationToken)
    {
        _templateManager.Initialize();

        return ValueTask.CompletedTask;
    }

    protected override EmptyResponse GetResponse()
    {
        return new EmptyResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(EmptyRequest data)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager>(),
            new List<IManager> { _templateManager }
        );
    }
}