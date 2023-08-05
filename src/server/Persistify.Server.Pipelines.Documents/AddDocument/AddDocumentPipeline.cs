﻿using System.Collections.Generic;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Persistence.Core.Transactions;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Documents.AddDocument.Stages;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Documents.AddDocument;

public class AddDocumentPipeline : Pipeline<AddDocumentPipelineContext, AddDocumentRequest, AddDocumentResponse>
{
    public AddDocumentPipeline(
        ILogger<AddDocumentPipeline> logger,
        ITransactionManager transactionManager,
        ISystemClock systemClock,
        StaticValidationStage<AddDocumentPipelineContext, AddDocumentRequest, AddDocumentResponse>
            staticValidationStage,
        AddDocumentToDocumentManagerStage addDocumentToDocumentManagerStage
    ) : base(logger, transactionManager, systemClock)
    {
        PipelineStages = new PipelineStage<AddDocumentPipelineContext, AddDocumentRequest, AddDocumentResponse>[]
        {
            staticValidationStage, addDocumentToDocumentManagerStage
        };
    }

    protected override IEnumerable<PipelineStage<AddDocumentPipelineContext, AddDocumentRequest, AddDocumentResponse>>
        PipelineStages
    {
        get;
    }

    protected override AddDocumentPipelineContext CreateContext(AddDocumentRequest request)
    {
        return new AddDocumentPipelineContext(request);
    }

    protected override AddDocumentResponse CreateResponse(AddDocumentPipelineContext context)
    {
        return new AddDocumentResponse(context.DocumentId ?? throw new PipelineException());
    }

    protected override Transaction CreateTransaction(AddDocumentPipelineContext context)
    {
        return new Transaction(false, new Dictionary<int, bool> { { context.Request.TemplateId, true } });
    }
}
