using System.Threading.Tasks;
using Persistify.Server.Fts.Analysis.Abstractions;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Templates.CreateTemplate.Stages;

public class
    CheckAnalyzersAvailabilityStage : PipelineStage<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>
{
    private readonly IAnalyzerFactory _analyzerFactory;
    private readonly IAnalyzerPresetFactory _analyzerPresetFactory;
    private const string StageName = "CheckAnalyzersAvailability";
    public override string Name => StageName;

    public CheckAnalyzersAvailabilityStage(
        IAnalyzerFactory analyzerFactory,
        IAnalyzerPresetFactory analyzerPresetFactory
    )
    {
        _analyzerFactory = analyzerFactory;
        _analyzerPresetFactory = analyzerPresetFactory;
    }

    public override ValueTask<Result> ProcessAsync(CreateTemplatePipelineContext context)
    {
        var textFields = context.Request.TextFields;

        foreach (var textField in textFields)
        {
            if (textField.AnalyzerDescriptor is not null)
            {
                var result = _analyzerFactory.Validate(textField.AnalyzerDescriptor);
                if (result.IsFailure)
                {
                    return ValueTask.FromResult(result);
                }
            }
            else if (textField.AnalyzerPresetName is not null)
            {
                var result = _analyzerPresetFactory.Validate(textField.AnalyzerPresetName);
                if (result.IsFailure)
                {
                    return ValueTask.FromResult(result);
                }
            }
        }

        return ValueTask.FromResult(Result.Success);
    }

    public override ValueTask<Result> RollbackAsync(CreateTemplatePipelineContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }
}
