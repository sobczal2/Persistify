using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Internal;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Internal;

public class SetupFileSystemRequestValidator : Validator<SetupFileSystemRequest>
{
    public override ValueTask<Result> ValidateNotNullAsync(SetupFileSystemRequest value)
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
