using System.Collections.Generic;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Persistence.Core.Transactions;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Documents.GetDocument.Stages;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Documents.GetDocument;

public class GetDocumentPipeline : Pipeline<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>
{
    public GetDocumentPipeline(
        ILogger<GetDocumentPipeline> logger,
        ITransactionManager transactionManager,
        ISystemClock systemClock,
        StaticValidationStage<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>
            staticValidationStage,
        GetDocumentFromDocumentManagerStage getDocumentFromDocumentManagerStage
    ) : base(
        logger,
        transactionManager,
        systemClock
    )
    {
        PipelineStages =
            new PipelineStage<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>[]
            {
                staticValidationStage, getDocumentFromDocumentManagerStage
            };
    }

    protected override IEnumerable<PipelineStage<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>>
        PipelineStages
    {
        get;
    }

    protected override GetDocumentPipelineContext CreateContext(GetDocumentRequest request)
    {
        return new GetDocumentPipelineContext(request);
    }

    protected override GetDocumentResponse CreateResponse(GetDocumentPipelineContext context)
    {
        return new GetDocumentResponse(context.Document ?? throw new PipelineException());
    }

    protected override Transaction CreateTransaction(GetDocumentPipelineContext context)
    {
        return new Transaction(false, new Dictionary<int, bool> { { context.Request.TemplateId, false } });
    }
}
