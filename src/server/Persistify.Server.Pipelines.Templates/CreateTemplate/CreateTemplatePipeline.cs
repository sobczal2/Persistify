using System;
using System.Collections.Generic;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Persistence.Core.Transactions;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Exceptions;
using Persistify.Server.Pipelines.Templates.CreateTemplate.Stages;

namespace Persistify.Server.Pipelines.Templates.CreateTemplate;

public class
    CreateTemplatePipeline : Pipeline<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>
{
    public CreateTemplatePipeline(
        ILogger<CreateTemplatePipeline> logger,
        ISystemClock systemClock,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        StaticValidationStage<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>
            staticValidationStage,
        CheckAnalyzersAvailabilityStage checkAnalyzersAvailabilityStage,
        AddTemplateToTemplateManagerStage addTemplateToTemplateManagerStage,
        InitializeTemplateInTypeManagersStage initializeTemplateInTypeManagersStage) : base(logger, transactionManager,
        systemClock)
    {
        PipelineStages =
            new PipelineStage<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>[]
            {
                staticValidationStage, checkAnalyzersAvailabilityStage, addTemplateToTemplateManagerStage,
                initializeTemplateInTypeManagersStage
            };
    }

    protected override
        IEnumerable<PipelineStage<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>>
        PipelineStages
    {
        get;
    }

    protected override CreateTemplatePipelineContext CreateContext(CreateTemplateRequest request)
    {
        return new CreateTemplatePipelineContext(request);
    }

    protected override CreateTemplateResponse CreateResponse(CreateTemplatePipelineContext context)
    {
        return new CreateTemplateResponse(context.Template?.Id ?? throw new PipelineException());
    }

    protected override Transaction CreateTransaction(CreateTemplatePipelineContext context)
    {
        return new Transaction(true, new Dictionary<int, bool>());
    }
}
