using System.Threading.Tasks;
using Persistify.Dtos.Templates.Common;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Management.Managers.PresetAnalyzerDescriptors;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.Common;

public class PresetAnalyzerDescriptorDtoValidator : Validator<PresetAnalyzerDescriptorDto>
{
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;

    public PresetAnalyzerDescriptorDtoValidator(
        IPresetAnalyzerManager presetAnalyzerManager
    )
    {
        _presetAnalyzerManager = presetAnalyzerManager;
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

        if (!_presetAnalyzerManager.Exists(value.PresetName))
        {
            PropertyName.Push(nameof(PresetAnalyzerDescriptorDto.PresetName));
            return ValueTask.FromResult<Result>(DynamicValidationException(TemplateErrorMessages.PresetAnalyzerNotFound));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
