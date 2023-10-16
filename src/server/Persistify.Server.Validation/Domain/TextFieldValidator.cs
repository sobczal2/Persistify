using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Helpers.Results;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Domain;

public class TextFieldValidator : Validator<TextField>
{
    private readonly IValidator<FullAnalyzerDescriptor> _analyzerDescriptorValidator;
    private readonly IValidator<PresetAnalyzerDescriptor> _presetAnalyzerDescriptorValidator;

    public TextFieldValidator(IValidator<FullAnalyzerDescriptor> analyzerDescriptorValidator, IValidator<PresetAnalyzerDescriptor> presetAnalyzerDescriptorValidator)
    {
        _analyzerDescriptorValidator = analyzerDescriptorValidator;
        _presetAnalyzerDescriptorValidator = presetAnalyzerDescriptorValidator;
        _analyzerDescriptorValidator.PropertyName = PropertyName;
        _presetAnalyzerDescriptorValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(TextField));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(TextField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyName.Push(nameof(TextField.Name));
            return StaticValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.Name.Length > 64)
        {
            PropertyName.Push(nameof(TextField.Name));
            return StaticValidationException(SharedErrorMessages.ValueTooLong);
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.AnalyzerDescriptor is null)
        {
            PropertyName.Push(nameof(TextField.AnalyzerDescriptor));
            return StaticValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.AnalyzerDescriptor is PresetAnalyzerDescriptor presetAnalyzerDescriptor)
        {
            PropertyName.Push(nameof(TextField.AnalyzerDescriptor));
            var presetAnalyzerDescriptorValidationResult =
                await _presetAnalyzerDescriptorValidator.ValidateAsync(presetAnalyzerDescriptor);
            PropertyName.Pop();
            if (presetAnalyzerDescriptorValidationResult.Failure)
            {
                return presetAnalyzerDescriptorValidationResult;
            }
        }
        else if (value.AnalyzerDescriptor is FullAnalyzerDescriptor fullAnalyzerDescriptor)
        {
            PropertyName.Push(nameof(TextField.AnalyzerDescriptor));
            var analyzerDescriptorValidationResult =
                await _analyzerDescriptorValidator.ValidateAsync(fullAnalyzerDescriptor);
            PropertyName.Pop();
            if (analyzerDescriptorValidationResult.Failure)
            {
                return analyzerDescriptorValidationResult;
            }
        }

        return Result.Ok;
    }
}
