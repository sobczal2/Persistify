using Persistify.Domain.Templates;
using Persistify.Helpers.Common;
using Persistify.Helpers.ErrorHandling;
using Persistify.Validation.Common;

namespace Persistify.Validation.Domain;

public class TextFieldValidator : IValidator<TextField>
{
    public string ErrorPrefix { get; set; }

    public TextFieldValidator()
    {
        ErrorPrefix = "TextField";
    }

    public Result Validate(TextField value)
    {
        if(string.IsNullOrEmpty(value.Name))
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name is required");
        }

        if(value.Name.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name has a maximum length of 64 characters");
        }

        if(!LogicHelpers.Xor(value.AnalyzerPresetName is null, value.AnalyzerDescriptor is null))
        {
            return new ValidationException($"{ErrorPrefix}.AnalyzerPreset", "Either AnalyzerPresetName or AnalyzerDescriptor must be set");
        }

        if(value.AnalyzerPresetName is not null && value.AnalyzerPresetName.Length == 0)
        {
            return new ValidationException($"{ErrorPrefix}.AnalyzerPreset", "AnalyzerPresetName cannot be empty");
        }

        if(value.AnalyzerPresetName is not null && value.AnalyzerPresetName.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.AnalyzerPreset", "AnalyzerPresetName has a maximum length of 64 characters");
        }
    }
}
