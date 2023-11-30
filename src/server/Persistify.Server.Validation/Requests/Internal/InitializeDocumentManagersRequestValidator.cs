using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Internal;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Internal;

public class InitializeDocumentManagersRequestValidator
    : Validator<InitializeDocumentManagersRequest>
{
    public override ValueTask<Result> ValidateNotNullAsync(
        InitializeDocumentManagersRequest value
    )
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
