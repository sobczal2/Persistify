using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Exceptions;
using Persistify.Server.Pipelines.Templates.ListTemplates.Stages;

namespace Persistify.Server.Pipelines.Templates.ListTemplates;

public class ListTemplatesPipeline : Pipeline<ListTemplatesPipelineContext, ListTemplatesRequest, ListTemplatesResponse>
{
    public ListTemplatesPipeline(
        ILogger<ListTemplatesPipeline> logger,
        ITransactionManager transactionManager,
        StaticValidationStage<ListTemplatesPipelineContext, ListTemplatesRequest, ListTemplatesResponse>
            staticValidationStage,
        FetchTemplatesFromTemplateManagerStage fetchTemplatesFromTemplateManagerStage,
        ApplyPaginationStage applyPaginationStage) : base(logger, transactionManager)
    {
        PipelineStages = new PipelineStage<ListTemplatesPipelineContext, ListTemplatesRequest, ListTemplatesResponse>[]
        {
            staticValidationStage,
            fetchTemplatesFromTemplateManagerStage,
            applyPaginationStage
        };
    }

    protected override
        IEnumerable<PipelineStage<ListTemplatesPipelineContext, ListTemplatesRequest, ListTemplatesResponse>>
        PipelineStages
    {
        get;
    }

    protected override ListTemplatesPipelineContext CreateContext(ListTemplatesRequest request)
    {
        return new ListTemplatesPipelineContext(request);
    }

    protected override ListTemplatesResponse CreateResponse(ListTemplatesPipelineContext context)
    {
        return new ListTemplatesResponse(context.Templates ?? throw new PipelineException(), context.TotalCount);
    }

    protected override (bool write, bool global, IEnumerable<int> templateIds) GetTransactionInfo(
        ListTemplatesPipelineContext context)
    {
        return (false, true, Array.Empty<int>());
    }
}
