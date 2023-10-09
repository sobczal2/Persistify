using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Helpers.Results;
using Persistify.Server.Fts.Analysis.Abstractions;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Domain;

public class PresetAnalyzerDescriptorValidator : Validator<PresetAnalyzerDescriptor>
{
    private readonly IAnalyzerPresetFactory _analyzerPresetFactory;

    public PresetAnalyzerDescriptorValidator(
        IAnalyzerPresetFactory analyzerPresetFactory
        )
    {
        _analyzerPresetFactory = analyzerPresetFactory;
        PropertyName.Push(nameof(PresetAnalyzerDescriptor));
    }
    public override ValueTask<Result> ValidateNotNullAsync(PresetAnalyzerDescriptor value)
    {
        if (string.IsNullOrEmpty(value.PresetName))
        {
            PropertyName.Push(nameof(PresetAnalyzerDescriptor.PresetName));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.PresetName.Length > 64)
        {
            PropertyName.Push(nameof(PresetAnalyzerDescriptor.PresetName));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (_analyzerPresetFactory.Validate(value.PresetName).Failure)
        {
            PropertyName.Push(nameof(PresetAnalyzerDescriptor.PresetName));
            return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.PresetNotFound));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
