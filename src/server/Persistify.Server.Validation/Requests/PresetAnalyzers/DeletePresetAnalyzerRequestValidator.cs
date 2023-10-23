using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Management.Managers.PresetAnalyzers;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.PresetAnalyzers;

public class DeletePresetAnalyzerRequestValidator : Validator<DeletePresetAnalyzerRequest>
{
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;

    public DeletePresetAnalyzerRequestValidator(
        IPresetAnalyzerManager presetAnalyzerManager
    )
    {
        _presetAnalyzerManager = presetAnalyzerManager;
        PropertyName.Push(nameof(DeletePresetAnalyzerRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(DeletePresetAnalyzerRequest value)
    {
        if (string.IsNullOrEmpty(value.PresetAnalyzerName))
        {
            PropertyName.Push(nameof(DeletePresetAnalyzerRequest.PresetAnalyzerName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.PresetAnalyzerName.Length > 64)
        {
            PropertyName.Push(nameof(DeletePresetAnalyzerRequest.PresetAnalyzerName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (!_presetAnalyzerManager.Exists(value.PresetAnalyzerName))
        {
            PropertyName.Push(nameof(DeletePresetAnalyzerRequest.PresetAnalyzerName));
            return ValueTask.FromResult<Result>(
                DynamicValidationException(PresetAnalyzerErrorMessages.PresetAnalyzerNotFound));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
