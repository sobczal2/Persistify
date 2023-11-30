using System.Threading.Tasks;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Helpers.Results;
using Persistify.Server.Domain.Templates;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.Common;

public class FullAnalyzerDtoValidator : Validator<FullAnalyzerDto>
{
    private readonly IAnalyzerExecutorLookup _analyzerExecutorLookup;

    public FullAnalyzerDtoValidator(IAnalyzerExecutorLookup analyzerExecutorLookup)
    {
        _analyzerExecutorLookup = analyzerExecutorLookup;
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
                return ValueTask.FromResult<Result>(
                    StaticValidationException(TemplateErrorMessages.NameEmpty)
                );
            }

            if (characterSetName.Length > 64)
            {
                PropertyName.Push($"{nameof(FullAnalyzerDto.CharacterSetNames)}[{count}]");
                return ValueTask.FromResult<Result>(
                    StaticValidationException(SharedErrorMessages.ValueTooLong)
                );
            }

            count++;
        }

        count = 0;
        foreach (var characterFilterName in value.CharacterFilterNames)
        {
            if (string.IsNullOrEmpty(characterFilterName))
            {
                PropertyName.Push($"{nameof(FullAnalyzerDto.CharacterSetNames)}[{count}]");
                return ValueTask.FromResult<Result>(
                    StaticValidationException(TemplateErrorMessages.NameEmpty)
                );
            }

            if (characterFilterName.Length > 64)
            {
                PropertyName.Push($"{nameof(FullAnalyzerDto.CharacterSetNames)}[{count}]");
                return ValueTask.FromResult<Result>(
                    StaticValidationException(SharedErrorMessages.ValueTooLong)
                );
            }

            count++;
        }

        if (string.IsNullOrEmpty(value.TokenizerName))
        {
            PropertyName.Push(nameof(FullAnalyzerDto.TokenizerName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(TemplateErrorMessages.NameEmpty)
            );
        }

        if (value.TokenizerName.Length > 64)
        {
            PropertyName.Push(nameof(FullAnalyzerDto.TokenizerName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueTooLong)
            );
        }

        count = 0;
        foreach (var tokenFilterName in value.TokenFilterNames)
        {
            if (string.IsNullOrEmpty(tokenFilterName))
            {
                PropertyName.Push($"{nameof(FullAnalyzerDto.TokenFilterNames)}[{count}]");
                return ValueTask.FromResult<Result>(
                    StaticValidationException(TemplateErrorMessages.NameEmpty)
                );
            }

            if (tokenFilterName.Length > 64)
            {
                PropertyName.Push($"{nameof(FullAnalyzerDto.TokenFilterNames)}[{count}]");
                return ValueTask.FromResult<Result>(
                    StaticValidationException(SharedErrorMessages.ValueTooLong)
                );
            }

            count++;
        }

        var analyzerFactoryResult = _analyzerExecutorLookup.Validate(value);
        if (analyzerFactoryResult.Failure)
        {
            return ValueTask.FromResult(analyzerFactoryResult);
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
