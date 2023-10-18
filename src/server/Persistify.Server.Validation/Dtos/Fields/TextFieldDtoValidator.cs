using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Dtos.Templates.Common;
using Persistify.Dtos.Templates.Fields;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.Fields;

public class TextFieldDtoValidator : Validator<TextFieldDto>
{
    private readonly IValidator<FullAnalyzerDescriptorDto> _analyzerDescriptorDtoValidator;
    private readonly IValidator<PresetAnalyzerDescriptorDto> _presetAnalyzerDescriptorDtoValidator;

    public TextFieldDtoValidator(
        IValidator<FullAnalyzerDescriptorDto> analyzerDescriptorDtoValidator,
        IValidator<PresetAnalyzerDescriptorDto> presetAnalyzerDescriptorDtoValidator
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
        if (value.AnalyzerDescriptor is null)
        {
            PropertyName.Push(nameof(TextField.AnalyzerDescriptor));
            return StaticValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.AnalyzerDescriptor is PresetAnalyzerDescriptorDto presetAnalyzerDescriptor)
        {
            PropertyName.Push(nameof(TextField.AnalyzerDescriptor));
            var presetAnalyzerDescriptorValidationResult =
                await _presetAnalyzerDescriptorDtoValidator.ValidateAsync(presetAnalyzerDescriptor);
            PropertyName.Pop();
            if (presetAnalyzerDescriptorValidationResult.Failure)
            {
                return presetAnalyzerDescriptorValidationResult;
            }
        }
        else if (value.AnalyzerDescriptor is FullAnalyzerDescriptorDto fullAnalyzerDescriptor)
        {
            PropertyName.Push(nameof(TextField.AnalyzerDescriptor));
            var analyzerDescriptorValidationResult =
                await _analyzerDescriptorDtoValidator.ValidateAsync(fullAnalyzerDescriptor);
            PropertyName.Pop();
            if (analyzerDescriptorValidationResult.Failure)
            {
                return analyzerDescriptorValidationResult;
            }
        }

        return Result.Ok;
    }
}
