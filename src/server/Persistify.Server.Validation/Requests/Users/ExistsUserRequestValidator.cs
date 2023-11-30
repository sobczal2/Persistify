using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Users;

public class ExistsUserRequestValidator : Validator<ExistsUserRequest>
{
    public ExistsUserRequestValidator()
    {
        PropertyName.Push(nameof(GetUserRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(
        ExistsUserRequest value
    )
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(GetUserRequest.Username));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueNull)
            );
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(GetUserRequest.Username));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueTooLong)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
