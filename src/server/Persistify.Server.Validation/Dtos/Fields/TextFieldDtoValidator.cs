using System.Threading.Tasks;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Fields;
using Persistify.Helpers.Results;
using Persistify.Server.Domain.Templates;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.Fields;

public class TextFieldDtoValidator : Validator<TextFieldDto>
{
    private readonly IValidator<FullAnalyzerDto> _analyzerDescriptorDtoValidator;
    private readonly IValidator<PresetNameAnalyzerDto> _presetAnalyzerDescriptorDtoValidator;

    public TextFieldDtoValidator(
        IValidator<FullAnalyzerDto> analyzerDescriptorDtoValidator,
        IValidator<PresetNameAnalyzerDto> presetAnalyzerDescriptorDtoValidator
        )
    {
        _analyzerDescriptorDtoValidator = analyzerDescriptorDtoValidator;
        _analyzerDescriptorDtoValidator.PropertyName = PropertyName;
        _presetAnalyzerDescriptorDtoValidator = presetAnalyzerDescriptorDtoValidator;
        _presetAnalyzerDescriptorDtoValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(TextField));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(TextFieldDto value)
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
        if (value.Analyzer is null)
        {
            PropertyName.Push(nameof(TextField.Analyzer));
            return StaticValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.Analyzer is PresetNameAnalyzerDto presetAnalyzerDescriptor)
        {
            PropertyName.Push(nameof(TextField.Analyzer));
            var presetAnalyzerDescriptorValidationResult =
                await _presetAnalyzerDescriptorDtoValidator.ValidateAsync(presetAnalyzerDescriptor);
            PropertyName.Pop();
            if (presetAnalyzerDescriptorValidationResult.Failure)
            {
                return presetAnalyzerDescriptorValidationResult;
            }
        }
        else if (value.Analyzer is FullAnalyzerDto fullAnalyzerDescriptor)
        {
            PropertyName.Push(nameof(TextField.Analyzer));
            var analyzerDescriptorValidationResult =
                await _analyzerDescriptorDtoValidator.ValidateAsync(fullAnalyzerDescriptor);
            PropertyName.Pop();
            if (analyzerDescriptorValidationResult.Failure)
            {
                return analyzerDescriptorValidationResult;
            }
        }
        else
        {
            PropertyName.Push(nameof(TextField.Analyzer));
            return StaticValidationException(TemplateErrorMessages.InvalidAnalyzerDescriptor);
        }

        return Result.Ok;
    }
}
