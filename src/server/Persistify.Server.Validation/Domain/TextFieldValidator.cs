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

    public TextFieldValidator(IValidator<FullAnalyzerDescriptor> analyzerDescriptorValidator)
    {
        _analyzerDescriptorValidator = analyzerDescriptorValidator;
        _analyzerDescriptorValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(TextField));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(TextField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyName.Push(nameof(TextField.Name));
            return ValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.Name.Length > 64)
        {
            PropertyName.Push(nameof(TextField.Name));
            return ValidationException(SharedErrorMessages.ValueTooLong);
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.AnalyzerDescriptor is null)
        {
            PropertyName.Push(nameof(TextField.AnalyzerDescriptor));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.AnalyzerDescriptor is PresetAnalyzerDescriptor presetAnalyzerDescriptor)
        {
            if (string.IsNullOrEmpty(presetAnalyzerDescriptor.PresetName))
            {
                PropertyName.Push(
                    $"{nameof(TextField.AnalyzerDescriptor)}.{nameof(PresetAnalyzerDescriptor.PresetName)}");
                return ValidationException(TemplateErrorMessages.NameEmpty);
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
