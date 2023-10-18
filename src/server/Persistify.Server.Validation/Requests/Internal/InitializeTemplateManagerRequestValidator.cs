using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Internal;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Internal;

public class InitializeTemplateManagerRequestValidator : Validator<InitializeTemplateManagerRequest>
{
    public override ValueTask<Result> ValidateNotNullAsync(InitializeTemplateManagerRequest value)
    {
        return ValueTask.FromResult(Result.Ok);
    }
}
