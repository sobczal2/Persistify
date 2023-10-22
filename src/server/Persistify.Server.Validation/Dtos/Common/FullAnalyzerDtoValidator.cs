using System.Threading.Tasks;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Helpers.Results;
using Persistify.Server.Domain.Templates;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Exceptions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.Common;

// TODO: Add tests
public class FullAnalyzeRDtoValidator : Validator<FullAnalyzerDto>
{
    private readonly IAnalyzerExecutorFactory _analyzerExecutorFactory;

    public FullAnalyzeRDtoValidator(
        IAnalyzerExecutorFactory analyzerExecutorFactory
    )
    {
        _analyzerExecutorFactory = analyzerExecutorFactory;
        PropertyName.Push(nameof(Analyzer));
    }

    public override ValueTask<Result> ValidateNotNullAsync(FullAnalyzerDto value)
    {
        var count = 0;
        foreach (var characterSetName in value.CharacterSetNames)
        {
            if (string.IsNullOrEmpty(characterSetName))
            {
                PropertyName.Push($"{nameof(FullAnalyzerDto.CharacterSetNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(TemplateErrorMessages.NameEmpty));
            }

            if (characterSetName.Length > 64)
            {
                PropertyName.Push($"{nameof(FullAnalyzerDto.CharacterSetNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
            }

            count++;
        }

        count = 0;
        foreach (var characterFilterName in value.CharacterFilterNames)
        {
            if (string.IsNullOrEmpty(characterFilterName))
            {
                PropertyName.Push($"{nameof(FullAnalyzerDto.CharacterSetNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(TemplateErrorMessages.NameEmpty));
            }

            if (characterFilterName.Length > 64)
            {
                PropertyName.Push($"{nameof(FullAnalyzerDto.CharacterSetNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
            }

            count++;
        }

        if (string.IsNullOrEmpty(value.TokenizerName))
        {
            PropertyName.Push(nameof(FullAnalyzerDto.TokenizerName));
            return ValueTask.FromResult<Result>(StaticValidationException(TemplateErrorMessages.NameEmpty));
        }

        if (value.TokenizerName.Length > 64)
        {
            PropertyName.Push(nameof(FullAnalyzerDto.TokenizerName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        count = 0;
        foreach (var tokenFilterName in value.TokenFilterNames)
        {
            if (string.IsNullOrEmpty(tokenFilterName))
            {
                PropertyName.Push($"{nameof(FullAnalyzerDto.TokenFilterNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(TemplateErrorMessages.NameEmpty));
            }

            if (tokenFilterName.Length > 64)
            {
                PropertyName.Push($"{nameof(FullAnalyzerDto.TokenFilterNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
            }

            count++;
        }

        var analyzerFactoryResult = _analyzerExecutorFactory.Validate(value);
        if (analyzerFactoryResult.Failure)
        {
            switch (analyzerFactoryResult.Exception)
            {
                case UnsupportedCharacterSetException unsupportedCharacterFilterException:
                    PropertyName.Push($"{nameof(FullAnalyzerDto.CharacterSetNames)}");
                    return ValueTask.FromResult<Result>(
                        DynamicValidationException(unsupportedCharacterFilterException.Message));
                case UnsupportedTokenizerException unsupportedTokenizerException:
                    PropertyName.Push(nameof(FullAnalyzerDto.TokenizerName));
                    return ValueTask.FromResult<Result>(
                        DynamicValidationException(unsupportedTokenizerException.Message));
                case UnsupportedTokenFilterException unsupportedTokenFilterException:
                    PropertyName.Push($"{nameof(FullAnalyzerDto.TokenFilterNames)}");
                    return ValueTask.FromResult<Result>(
                        DynamicValidationException(unsupportedTokenFilterException.Message));
                default:
                    throw new InternalPersistifyException();
            }
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
