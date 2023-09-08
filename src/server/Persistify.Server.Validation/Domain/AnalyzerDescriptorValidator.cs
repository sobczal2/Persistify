using System.Text;
using Microsoft.Extensions.ObjectPool;
using Persistify.Domain.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Domain;

public class AnalyzerDescriptorValidator : Validator<AnalyzerDescriptor>
{
    public AnalyzerDescriptorValidator()
    {
        PropertyNames.Push(nameof(AnalyzerDescriptor));
    }

    public override Result Validate(AnalyzerDescriptor value)
    {
        if (value.CharacterFilterNames.Count > 0)
        {
            for (var i = 0; i < value.CharacterFilterNames.Count; i++)
            {
                if (string.IsNullOrEmpty(value.CharacterFilterNames[i]))
                {
                    PropertyNames.Push($"{nameof(AnalyzerDescriptor.CharacterFilterNames)}[{i}]");
                    return ValidationException(TemplateErrorMessages.NameEmpty);
                }

                if (value.CharacterFilterNames[i].Length > 64)
                {
                    PropertyNames.Push($"{nameof(AnalyzerDescriptor.CharacterFilterNames)}[{i}]");
                    return ValidationException(TemplateErrorMessages.NameTooLong);
                }
            }
        }

        if (string.IsNullOrEmpty(value.TokenizerName))
        {
            PropertyNames.Push(nameof(AnalyzerDescriptor.TokenizerName));
            return ValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.TokenizerName.Length > 64)
        {
            PropertyNames.Push(nameof(AnalyzerDescriptor.TokenizerName));
            return ValidationException(TemplateErrorMessages.NameTooLong);
        }

        if (value.TokenFilterNames.Count > 0)
        {
            for (var i = 0; i < value.TokenFilterNames.Count; i++)
            {
                if (string.IsNullOrEmpty(value.TokenFilterNames[i]))
                {
                    PropertyNames.Push($"{nameof(AnalyzerDescriptor.TokenFilterNames)}[{i}]");
                    return ValidationException(TemplateErrorMessages.NameEmpty);
                }

                if (value.TokenFilterNames[i].Length > 64)
                {
                    PropertyNames.Push($"{nameof(AnalyzerDescriptor.TokenFilterNames)}[{i}]");
                    return ValidationException(TemplateErrorMessages.NameTooLong);
                }
            }
        }

        return Result.Ok;
    }
}
