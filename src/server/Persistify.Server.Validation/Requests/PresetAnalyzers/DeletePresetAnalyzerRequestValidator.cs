using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.PresetAnalyzers;

public class DeletePresetAnalyzerRequestValidator : Validator<DeletePresetAnalyzerRequest>
{
    public DeletePresetAnalyzerRequestValidator()
    {
        PropertyName.Push(nameof(DeletePresetAnalyzerRequest));
    }
    public override ValueTask<Result> ValidateNotNullAsync(DeletePresetAnalyzerRequest value)
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
