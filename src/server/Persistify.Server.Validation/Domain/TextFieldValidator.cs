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
        _analyzerDescriptorValidator.PropertyNames = PropertyNames;
        PropertyNames.Push(nameof(TextField));
    }

    public override Result Validate(TextField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyNames.Push(nameof(TextField.Name));
            return ValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.Name.Length > 64)
        {
            PropertyNames.Push(nameof(TextField.Name));
            return ValidationException(TemplateErrorMessages.NameTooLong);
        }

        var analyzerPresetNameNull = value.AnalyzerPresetName is null;
        var analyzerDescriptorNull = value.AnalyzerDescriptor is null;
        if ((analyzerPresetNameNull && analyzerDescriptorNull) || (!analyzerPresetNameNull && !analyzerDescriptorNull))
        {
            PropertyNames.Push(nameof(TextField.AnalyzerPresetName));
            return ValidationException(TemplateErrorMessages.AnalyzerPresetNameOrAnalyzerDescriptorRequired);
        }

        if (value.AnalyzerPresetName is not null && value.AnalyzerPresetName.Length == 0)
        {
            PropertyNames.Push(nameof(TextField.AnalyzerPresetName));
            return ValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.AnalyzerPresetName is not null && value.AnalyzerPresetName.Length > 64)
        {
            PropertyNames.Push(nameof(TextField.AnalyzerPresetName));
            return ValidationException(TemplateErrorMessages.NameTooLong);
        }

        if (value.AnalyzerDescriptor is not null)
        {
            PropertyNames.Push(nameof(TextField.AnalyzerDescriptor));
            var analyzerDescriptorValidationResult = _analyzerDescriptorValidator.Validate(value.AnalyzerDescriptor);
            PropertyNames.Pop();
            if (analyzerDescriptorValidationResult.Failure)
            {
                return analyzerDescriptorValidationResult;
            }
        }

        return Result.Ok;
    }
}
