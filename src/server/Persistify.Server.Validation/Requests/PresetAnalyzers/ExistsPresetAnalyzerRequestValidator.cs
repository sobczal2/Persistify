using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.PresetAnalyzers;

public class ExistsPresetAnalyzerRequestValidator : Validator<ExistsPresetAnalyzerRequest>
{
    public ExistsPresetAnalyzerRequestValidator()
    {
        PropertyName.Push(nameof(ExistsPresetAnalyzerRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(ExistsPresetAnalyzerRequest value)
    {
        if (string.IsNullOrEmpty(value.PresetAnalyzerName))
        {
            PropertyName.Push(nameof(CreatePresetAnalyzerRequest.PresetAnalyzerName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueNull)
            );
        }

        if (value.PresetAnalyzerName.Length > 64)
        {
            PropertyName.Push(nameof(CreatePresetAnalyzerRequest.PresetAnalyzerName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueTooLong)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
