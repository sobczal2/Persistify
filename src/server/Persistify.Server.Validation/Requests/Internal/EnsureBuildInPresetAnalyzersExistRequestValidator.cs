using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Internal;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Internal;

public class EnsureBuildInPresetAnalyzersExistRequestValidator : Validator<EnsureBuildInPresetAnalyzersExistRequest>
{
    public override ValueTask<Result> ValidateNotNullAsync(EnsureBuildInPresetAnalyzersExistRequest value)
    {
        return new ValueTask<Result>(Result.Ok);
    }
}
