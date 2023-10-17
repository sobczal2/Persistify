using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

public class GetUserRequestValidator : Validator<GetUserRequest>
{
    public GetUserRequestValidator()
    {
        PropertyName.Push(nameof(GetUserRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(GetUserRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(GetUserRequest.Username));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(GetUserRequest.Username));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
