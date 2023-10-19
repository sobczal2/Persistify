using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.PresetAnalyzers;

public class ListPresetAnalyzersRequestValidator : Validator<ListPresetAnalyzersRequest>
{
    public ListPresetAnalyzersRequestValidator()
    {
        PropertyName.Push(nameof(ListPresetAnalyzersRequest));
    }
    public override ValueTask<Result> ValidateNotNullAsync(ListPresetAnalyzersRequest value)
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
