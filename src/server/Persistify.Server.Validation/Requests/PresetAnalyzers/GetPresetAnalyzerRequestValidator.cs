using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.PresetAnalyzers;

public class GetPresetAnalyzerRequestValidator : Validator<GetPresetAnalyzerRequest>
{
    public GetPresetAnalyzerRequestValidator()
    {
        PropertyName.Push(nameof(GetPresetAnalyzerRequest));
    }
    public override ValueTask<Result> ValidateNotNullAsync(GetPresetAnalyzerRequest value)
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
