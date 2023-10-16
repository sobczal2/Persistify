using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Fts.Analysis.Abstractions;
using Persistify.Server.Fts.Analysis.Exceptions;
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
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.PresetName.Length > 64)
        {
            PropertyName.Push(nameof(PresetAnalyzerDescriptor.PresetName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        var analyzerPresetFactoryResult = _analyzerPresetFactory.Validate(value.PresetName);

        if (analyzerPresetFactoryResult.Failure)
        {
            switch (analyzerPresetFactoryResult.Exception)
            {
                case AnalyzerPresetNotFoundException analyzerPresetNotFoundException:
                    PropertyName.Push(nameof(PresetAnalyzerDescriptor.PresetName));
                    return ValueTask.FromResult<Result>(DynamicValidationException(analyzerPresetNotFoundException.Message));
                default:
                    throw new InternalPersistifyException();
            }
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
