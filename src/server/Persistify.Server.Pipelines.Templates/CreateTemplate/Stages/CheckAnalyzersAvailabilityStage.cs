﻿using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Fts.Analysis.Abstractions;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Templates.CreateTemplate.Stages;

public class
    CheckAnalyzersAvailabilityStage : PipelineStage<CreateTemplatePipelineContext, CreateTemplateRequest,
        CreateTemplateResponse>
{
    private const string StageName = "CheckAnalyzersAvailability";
    private readonly IAnalyzerFactory _analyzerFactory;
    private readonly IAnalyzerPresetFactory _analyzerPresetFactory;

    public CheckAnalyzersAvailabilityStage(
        ILogger<CheckAnalyzersAvailabilityStage> logger,
        IAnalyzerFactory analyzerFactory,
        IAnalyzerPresetFactory analyzerPresetFactory
    ) : base(logger)
    {
        _analyzerFactory = analyzerFactory;
        _analyzerPresetFactory = analyzerPresetFactory;
    }

    public override string Name => StageName;

    public override ValueTask ProcessAsync(CreateTemplatePipelineContext context)
    {
        var textFields = context.Request.TextFields;

        foreach (var textField in textFields)
        {
            if (textField.AnalyzerDescriptor is not null)
            {
                var result = _analyzerFactory.Validate(textField.AnalyzerDescriptor);
                if (result.IsFailure)
                {
                    throw result.Exception;
                }
            }
            else if (textField.AnalyzerPresetName is not null)
            {
                var result = _analyzerPresetFactory.Validate(textField.AnalyzerPresetName);
                if (result.IsFailure)
                {
                    throw result.Exception;
                }
            }
        }

        return ValueTask.CompletedTask;
    }
}
