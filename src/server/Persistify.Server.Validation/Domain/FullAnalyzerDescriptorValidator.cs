using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Helpers.Results;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Domain;

// TODO: Add tests
public class FullAnalyzerDescriptorValidator : Validator<FullAnalyzerDescriptor>
{
    public FullAnalyzerDescriptorValidator()
    {
        PropertyName.Push(nameof(AnalyzerDescriptor));
    }

    public override ValueTask<Result> ValidateNotNullAsync(FullAnalyzerDescriptor value)
    {
        if (value.CharacterFilterNames.Count > 0)
        {
            for (var i = 0; i < value.CharacterFilterNames.Count; i++)
            {
                if (string.IsNullOrEmpty(value.CharacterFilterNames[i]))
                {
                    PropertyName.Push($"{nameof(FullAnalyzerDescriptor.CharacterFilterNames)}[{i}]");
                    return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.NameEmpty));
                }

                if (value.CharacterFilterNames[i].Length > 64)
                {
                    PropertyName.Push($"{nameof(FullAnalyzerDescriptor.CharacterFilterNames)}[{i}]");
                    return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
                }
            }
        }

        if (string.IsNullOrEmpty(value.TokenizerName))
        {
            PropertyName.Push(nameof(FullAnalyzerDescriptor.TokenizerName));
            return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.NameEmpty));
        }

        if (value.TokenizerName.Length > 64)
        {
            PropertyName.Push(nameof(FullAnalyzerDescriptor.TokenizerName));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (value.TokenFilterNames.Count > 0)
        {
            for (var i = 0; i < value.TokenFilterNames.Count; i++)
            {
                if (string.IsNullOrEmpty(value.TokenFilterNames[i]))
                {
                    PropertyName.Push($"{nameof(FullAnalyzerDescriptor.TokenFilterNames)}[{i}]");
                    return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.NameEmpty));
                }

                if (value.TokenFilterNames[i].Length > 64)
                {
                    PropertyName.Push($"{nameof(FullAnalyzerDescriptor.TokenFilterNames)}[{i}]");
                    return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
                }
            }
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
