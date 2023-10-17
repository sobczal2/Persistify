using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Internal;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Internal;

public class InitializeUserManagerRequestValidator : Validator<InitializeUserManagerRequest>
{
    public override ValueTask<Result> ValidateNotNullAsync(InitializeUserManagerRequest value)
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
