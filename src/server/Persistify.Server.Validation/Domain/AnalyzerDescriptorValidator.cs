using Persistify.Domain.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Domain;

public class AnalyzerDescriptorValidator : IValidator<AnalyzerDescriptor>
{
    public AnalyzerDescriptorValidator()
    {
        ErrorPrefix = "AnalyzerDescriptor";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(AnalyzerDescriptor value)
    {
        if (value.CharacterFilterNames.Count > 0)
        {
            for (var i = 0; i < value.CharacterFilterNames.Count; i++)
            {
                if (string.IsNullOrEmpty(value.CharacterFilterNames[i]))
                {
                    return new ValidationException($"{ErrorPrefix}.CharacterFilterNames[{i}]",
                        "CharacterFilterName cannot be empty");
                }

                if (value.CharacterFilterNames[i].Length > 64)
                {
                    return new ValidationException($"{ErrorPrefix}.CharacterFilterNames[{i}]",
                        "CharacterFilterName has a maximum length of 64 characters");
                }
            }
        }

        if (string.IsNullOrEmpty(value.TokenizerName))
        {
            return new ValidationException($"{ErrorPrefix}.TokenizerName", "TokenizerName is required");
        }

        if (value.TokenizerName.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.TokenizerName",
                "TokenizerName has a maximum length of 64 characters");
        }

        if (value.TokenFilterNames.Count > 0)
        {
            for (var i = 0; i < value.TokenFilterNames.Count; i++)
            {
                if (string.IsNullOrEmpty(value.TokenFilterNames[i]))
                {
                    return new ValidationException($"{ErrorPrefix}.TokenFilterNames[{i}]",
                        "TokenFilterName cannot be empty");
                }

                if (value.TokenFilterNames[i].Length > 64)
                {
                    return new ValidationException($"{ErrorPrefix}.TokenFilterNames[{i}]",
                        "TokenFilterName has a maximum length of 64 characters");
                }
            }
        }

        return Result.Ok;
    }
}
