using System.Text;
using Microsoft.Extensions.ObjectPool;
using Persistify.Requests.Shared;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Shared;

public class PaginationValidator : Validator<Pagination>
{
    public PaginationValidator()
    {
        PropertyNames.Push(nameof(Pagination));
    }

    public override Result Validate(Pagination value)
    {
        if (value.PageNumber < 0)
        {
            PropertyNames.Push(nameof(value.PageNumber));
            return ValidationException(SharedErrorMessages.PageNumberLessThanZero);
        }

        if (value.PageSize <= 0)
        {
            PropertyNames.Push(nameof(value.PageSize));
            return ValidationException(SharedErrorMessages.PageSizeLessThanOrEqualToZero);
        }

        return Result.Ok;
    }
}
