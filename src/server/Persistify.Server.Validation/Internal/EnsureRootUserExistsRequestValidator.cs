using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Internal;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Internal;

public class EnsureRootUserExistsRequestValidator : Validator<EnsureRootUserExistsRequest>
{
    public override ValueTask<Result> ValidateNotNullAsync(EnsureRootUserExistsRequest value)
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
