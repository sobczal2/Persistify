using System.Threading.Tasks;
using Persistify.Requests.Shared;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Shared;

public class PaginationValidator : Validator<Pagination>
{
    public PaginationValidator()
    {
        PropertyName.Push(nameof(Pagination));
    }

    public override ValueTask<Result> ValidateNotNullAsync(Pagination value)
    {
        if (value.PageNumber < 0)
        {
            PropertyName.Push(nameof(value.PageNumber));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.PageNumberLessThanZero));
        }

        if (value.PageSize <= 0)
        {
            PropertyName.Push(nameof(value.PageSize));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.PageSizeLessThanOrEqualToZero));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
