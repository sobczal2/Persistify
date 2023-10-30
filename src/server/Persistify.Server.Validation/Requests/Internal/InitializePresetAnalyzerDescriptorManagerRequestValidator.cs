using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Internal;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Internal;

public class InitializePresetAnalyzerDescriptorManagerRequestValidator
    : Validator<InitializePresetAnalyzerManagerRequest>
{
    public override ValueTask<Result> ValidateNotNullAsync(
        InitializePresetAnalyzerManagerRequest value
    )
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
