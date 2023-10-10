using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Shared;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Shared;

public class EmptyRequestValidator : Validator<EmptyRequest>
{
    public EmptyRequestValidator()
    {
        PropertyName.Push(nameof(EmptyRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(EmptyRequest value)
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
