using Persistify.Domain.Templates;
using Persistify.Helpers.Common;
using Persistify.Helpers.ErrorHandling;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Domain;

public class TextFieldValidator : IValidator<TextField>
{
    private readonly IValidator<AnalyzerDescriptor> _analyzerDescriptorValidator;

    public TextFieldValidator(IValidator<AnalyzerDescriptor> analyzerDescriptorValidator)
    {
        _analyzerDescriptorValidator = analyzerDescriptorValidator;
        ErrorPrefix = "TextField";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(TextField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name is required");
        }

        if (value.Name.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name has a maximum length of 64 characters");
        }

        if (!LogicHelpers.Xor(value.AnalyzerPresetName is null, value.AnalyzerDescriptor is null))
        {
            return new ValidationException($"{ErrorPrefix}.AnalyzerPreset",
                "Either AnalyzerPresetName or AnalyzerDescriptor must be set");
        }

        if (value.AnalyzerPresetName is not null && value.AnalyzerPresetName.Length == 0)
        {
            return new ValidationException($"{ErrorPrefix}.AnalyzerPreset", "AnalyzerPresetName cannot be empty");
        }

        if (value.AnalyzerPresetName is not null && value.AnalyzerPresetName.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.AnalyzerPreset",
                "AnalyzerPresetName has a maximum length of 64 characters");
        }

        if (value.AnalyzerDescriptor is not null)
        {
            _analyzerDescriptorValidator.ErrorPrefix = $"{ErrorPrefix}.AnalyzerDescriptor";
            var analyzerDescriptorValidationResult = _analyzerDescriptorValidator.Validate(value.AnalyzerDescriptor);
            if (analyzerDescriptorValidationResult.IsFailure)
            {
                return analyzerDescriptorValidationResult;
            }
        }

        return Result.Success;
    }
}
