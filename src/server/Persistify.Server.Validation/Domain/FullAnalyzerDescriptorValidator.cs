using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Fts.Analysis.Abstractions;
using Persistify.Server.Fts.Analysis.Exceptions;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Domain;

// TODO: Add tests
public class FullAnalyzerDescriptorValidator : Validator<FullAnalyzerDescriptor>
{
    private readonly IAnalyzerFactory _analyzerFactory;

    public FullAnalyzerDescriptorValidator(
        IAnalyzerFactory analyzerFactory
    )
    {
        _analyzerFactory = analyzerFactory;
        PropertyName.Push(nameof(AnalyzerDescriptor));
    }

    public override ValueTask<Result> ValidateNotNullAsync(FullAnalyzerDescriptor value)
    {
        var count = 0;
        foreach (var characterFilterName in value.CharacterFilterNames)
        {
            if (string.IsNullOrEmpty(characterFilterName))
            {
                PropertyName.Push($"{nameof(FullAnalyzerDescriptor.CharacterFilterNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(TemplateErrorMessages.NameEmpty));
            }

            if (characterFilterName.Length > 64)
            {
                PropertyName.Push($"{nameof(FullAnalyzerDescriptor.CharacterFilterNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
            }

            count++;
        }

        if (string.IsNullOrEmpty(value.TokenizerName))
        {
            PropertyName.Push(nameof(FullAnalyzerDescriptor.TokenizerName));
            return ValueTask.FromResult<Result>(StaticValidationException(TemplateErrorMessages.NameEmpty));
        }

        if (value.TokenizerName.Length > 64)
        {
            PropertyName.Push(nameof(FullAnalyzerDescriptor.TokenizerName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        count = 0;
        foreach (var tokenFilterName in value.TokenFilterNames)
        {
            if (string.IsNullOrEmpty(tokenFilterName))
            {
                PropertyName.Push($"{nameof(FullAnalyzerDescriptor.TokenFilterNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(TemplateErrorMessages.NameEmpty));
            }

            if (tokenFilterName.Length > 64)
            {
                PropertyName.Push($"{nameof(FullAnalyzerDescriptor.TokenFilterNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
            }

            count++;
        }

        var analyzerFactoryResult = _analyzerFactory.Validate(value);
        if (analyzerFactoryResult.Failure)
        {
            switch (analyzerFactoryResult.Exception)
            {
                case UnsupportedCharacterFilterException unsupportedCharacterFilterException:
                    PropertyName.Push($"{nameof(FullAnalyzerDescriptor.CharacterFilterNames)}");
                    return ValueTask.FromResult<Result>(
                        DynamicValidationException(unsupportedCharacterFilterException.Message));
                case UnsupportedTokenizerException unsupportedTokenizerException:
                    PropertyName.Push(nameof(FullAnalyzerDescriptor.TokenizerName));
                    return ValueTask.FromResult<Result>(
                        DynamicValidationException(unsupportedTokenizerException.Message));
                case UnsupportedTokenFilterException unsupportedTokenFilterException:
                    PropertyName.Push($"{nameof(FullAnalyzerDescriptor.TokenFilterNames)}");
                    return ValueTask.FromResult<Result>(
                        DynamicValidationException(unsupportedTokenFilterException.Message));
                default:
                    throw new InternalPersistifyException();
            }
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
