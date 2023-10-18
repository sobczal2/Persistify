using System.Threading.Tasks;
using Persistify.Dtos.Templates.Common;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.Common;

public class PresetAnalyzerDescriptorDtoValidator : Validator<PresetAnalyzerDescriptorDto>
{

    public PresetAnalyzerDescriptorDtoValidator(
    )
    {
        PropertyName.Push(nameof(PresetAnalyzerDescriptorDto));
    }

    public override ValueTask<Result> ValidateNotNullAsync(PresetAnalyzerDescriptorDto value)
    {
        if (string.IsNullOrEmpty(value.PresetName))
        {
            PropertyName.Push(nameof(PresetAnalyzerDescriptorDto.PresetName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.PresetName.Length > 64)
        {
            PropertyName.Push(nameof(PresetAnalyzerDescriptorDto.PresetName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
