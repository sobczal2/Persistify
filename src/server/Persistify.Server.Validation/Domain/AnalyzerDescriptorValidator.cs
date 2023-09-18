using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Domain;

// TODO: Add tests
public class AnalyzerDescriptorValidator : Validator<AnalyzerDescriptor>
{
    public AnalyzerDescriptorValidator()
    {
        PropertyName.Push(nameof(AnalyzerDescriptor));
    }

    public override ValueTask<Result> ValidateNotNullAsync(AnalyzerDescriptor value)
    {
        if (value.CharacterFilterNames.Count > 0)
        {
            for (var i = 0; i < value.CharacterFilterNames.Count; i++)
            {
                if (string.IsNullOrEmpty(value.CharacterFilterNames[i]))
                {
                    PropertyName.Push($"{nameof(AnalyzerDescriptor.CharacterFilterNames)}[{i}]");
                    return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.NameEmpty));
                }

                if (value.CharacterFilterNames[i].Length > 64)
                {
                    PropertyName.Push($"{nameof(AnalyzerDescriptor.CharacterFilterNames)}[{i}]");
                    return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.NameTooLong));
                }
            }
        }

        if (string.IsNullOrEmpty(value.TokenizerName))
        {
            PropertyName.Push(nameof(AnalyzerDescriptor.TokenizerName));
            return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.NameEmpty));
        }

        if (value.TokenizerName.Length > 64)
        {
            PropertyName.Push(nameof(AnalyzerDescriptor.TokenizerName));
            return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.NameTooLong));
        }

        if (value.TokenFilterNames.Count > 0)
        {
            for (var i = 0; i < value.TokenFilterNames.Count; i++)
            {
                if (string.IsNullOrEmpty(value.TokenFilterNames[i]))
                {
                    PropertyName.Push($"{nameof(AnalyzerDescriptor.TokenFilterNames)}[{i}]");
                    return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.NameEmpty));
                }

                if (value.TokenFilterNames[i].Length > 64)
                {
                    PropertyName.Push($"{nameof(AnalyzerDescriptor.TokenFilterNames)}[{i}]");
                    return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.NameTooLong));
                }
            }
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
