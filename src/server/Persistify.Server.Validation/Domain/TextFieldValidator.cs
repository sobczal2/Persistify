using System.Text;
using Microsoft.Extensions.ObjectPool;
using Persistify.Domain.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Domain;

public class TextFieldValidator : Validator<TextField>
{
    private readonly IValidator<AnalyzerDescriptor> _analyzerDescriptorValidator;

    public TextFieldValidator(IValidator<AnalyzerDescriptor> analyzerDescriptorValidator)
    {
        _analyzerDescriptorValidator = analyzerDescriptorValidator;
        _analyzerDescriptorValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(TextField));
    }

    public override Result ValidateNotNull(TextField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyName.Push(nameof(TextField.Name));
            return ValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.Name.Length > 64)
        {
            PropertyName.Push(nameof(TextField.Name));
            return ValidationException(TemplateErrorMessages.NameTooLong);
        }

        var analyzerPresetNameNull = value.AnalyzerPresetName is null;
        var analyzerDescriptorNull = value.AnalyzerDescriptor is null;
        if ((analyzerPresetNameNull && analyzerDescriptorNull) || (!analyzerPresetNameNull && !analyzerDescriptorNull))
        {
            PropertyName.Push(nameof(TextField.AnalyzerPresetName));
            return ValidationException(TemplateErrorMessages.AnalyzerPresetNameOrAnalyzerDescriptorRequired);
        }

        if (value.AnalyzerPresetName is not null && value.AnalyzerPresetName.Length == 0)
        {
            PropertyName.Push(nameof(TextField.AnalyzerPresetName));
            return ValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.AnalyzerPresetName is not null && value.AnalyzerPresetName.Length > 64)
        {
            PropertyName.Push(nameof(TextField.AnalyzerPresetName));
            return ValidationException(TemplateErrorMessages.NameTooLong);
        }

        if (value.AnalyzerDescriptor is not null)
        {
            PropertyName.Push(nameof(TextField.AnalyzerDescriptor));
            var analyzerDescriptorValidationResult = _analyzerDescriptorValidator.Validate(value.AnalyzerDescriptor);
            PropertyName.Pop();
            if (analyzerDescriptorValidationResult.Failure)
            {
                return analyzerDescriptorValidationResult;
            }
        }

        return Result.Ok;
    }
}
