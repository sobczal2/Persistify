using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Dtos.Templates.Common;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Exceptions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.Common;

// TODO: Add tests
public class FullAnalyzerDescriptorDtoValidator : Validator<FullAnalyzerDescriptorDto>
{
    private readonly IAnalyzerFactory _analyzerFactory;

    public FullAnalyzerDescriptorDtoValidator(
        IAnalyzerFactory analyzerFactory
    )
    {
        _analyzerFactory = analyzerFactory;
        PropertyName.Push(nameof(AnalyzerDescriptor));
    }

    public override ValueTask<Result> ValidateNotNullAsync(FullAnalyzerDescriptorDto value)
    {
        var count = 0;
        foreach (var characterFilterName in value.CharacterFilterNames)
        {
            if (string.IsNullOrEmpty(characterFilterName))
            {
                PropertyName.Push($"{nameof(FullAnalyzerDescriptorDto.CharacterFilterNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(TemplateErrorMessages.NameEmpty));
            }

            if (characterFilterName.Length > 64)
            {
                PropertyName.Push($"{nameof(FullAnalyzerDescriptorDto.CharacterFilterNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
            }

            count++;
        }

        if (string.IsNullOrEmpty(value.TokenizerName))
        {
            PropertyName.Push(nameof(FullAnalyzerDescriptorDto.TokenizerName));
            return ValueTask.FromResult<Result>(StaticValidationException(TemplateErrorMessages.NameEmpty));
        }

        if (value.TokenizerName.Length > 64)
        {
            PropertyName.Push(nameof(FullAnalyzerDescriptorDto.TokenizerName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        count = 0;
        foreach (var tokenFilterName in value.TokenFilterNames)
        {
            if (string.IsNullOrEmpty(tokenFilterName))
            {
                PropertyName.Push($"{nameof(FullAnalyzerDescriptorDto.TokenFilterNames)}[{count}]");
                return ValueTask.FromResult<Result>(StaticValidationException(TemplateErrorMessages.NameEmpty));
            }

            if (tokenFilterName.Length > 64)
            {
                PropertyName.Push($"{nameof(FullAnalyzerDescriptorDto.TokenFilterNames)}[{count}]");
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
                    PropertyName.Push($"{nameof(FullAnalyzerDescriptorDto.CharacterFilterNames)}");
                    return ValueTask.FromResult<Result>(
                        DynamicValidationException(unsupportedCharacterFilterException.Message));
                case UnsupportedTokenizerException unsupportedTokenizerException:
                    PropertyName.Push(nameof(FullAnalyzerDescriptorDto.TokenizerName));
                    return ValueTask.FromResult<Result>(
                        DynamicValidationException(unsupportedTokenizerException.Message));
                case UnsupportedTokenFilterException unsupportedTokenFilterException:
                    PropertyName.Push($"{nameof(FullAnalyzerDescriptorDto.TokenFilterNames)}");
                    return ValueTask.FromResult<Result>(
                        DynamicValidationException(unsupportedTokenFilterException.Message));
                default:
                    throw new InternalPersistifyException();
            }
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
