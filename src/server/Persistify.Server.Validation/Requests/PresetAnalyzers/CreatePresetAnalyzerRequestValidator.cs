using System.Threading.Tasks;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Helpers.Results;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Management.Managers.PresetAnalyzers;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.PresetAnalyzers;

public class CreatePresetAnalyzerRequestValidator : Validator<CreatePresetAnalyzerRequest>
{
    private readonly IValidator<FullAnalyzerDto> _analyzerDtoValidator;
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;

    public CreatePresetAnalyzerRequestValidator(
        IPresetAnalyzerManager presetAnalyzerManager,
        IValidator<FullAnalyzerDto> analyzerDtoValidator
    )
    {
        _presetAnalyzerManager = presetAnalyzerManager;
        _analyzerDtoValidator = analyzerDtoValidator;
        analyzerDtoValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(CreatePresetAnalyzerRequest));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(CreatePresetAnalyzerRequest value)
    {
        if (string.IsNullOrEmpty(value.PresetAnalyzerName))
        {
            PropertyName.Push(nameof(CreatePresetAnalyzerRequest.PresetAnalyzerName));
            return StaticValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.PresetAnalyzerName.Length > 64)
        {
            PropertyName.Push(nameof(CreatePresetAnalyzerRequest.PresetAnalyzerName));
            return StaticValidationException(SharedErrorMessages.ValueTooLong);
        }

        if (_presetAnalyzerManager.Exists(value.PresetAnalyzerName))
        {
            PropertyName.Push(nameof(CreatePresetAnalyzerRequest.PresetAnalyzerName));
            return DynamicValidationException(PresetAnalyzerErrorMessages.PresetAnalyzerNameNotUnique);
        }

        PropertyName.Push(nameof(CreatePresetAnalyzerRequest.FullAnalyzerDto));
        var analyzerDtoValidationResult = await _analyzerDtoValidator.ValidateAsync(value.FullAnalyzerDto);
        PropertyName.Pop();
        if (analyzerDtoValidationResult.Failure)
        {
            return analyzerDtoValidationResult;
        }

        return Result.Ok;
    }
}
