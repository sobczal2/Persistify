using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.PresetAnalyzers;

public class CreatePresetAnalyzerRequestValidator : Validator<CreatePresetAnalyzerRequest>
{
    public CreatePresetAnalyzerRequestValidator()
    {
        PropertyName.Push(nameof(CreatePresetAnalyzerRequest));
    }
    public override ValueTask<Result> ValidateNotNullAsync(CreatePresetAnalyzerRequest value)
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
