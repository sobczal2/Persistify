using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Common;

namespace Persistify.Validation.Common;

public class PaginationValidator : IValidator<Pagination>
{
    public Result<Unit> Validate(Pagination value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException("Pagination", "Pagination is null");
        }

        if (value.PageSize < 1)
        {
            return new ValidationException("Pagination.PageSize", "PageSize must be greater than 0");
        }

        if (value.PageNumber < 0)
        {
            return new ValidationException("Pagination.PageNumber", "PageNumber must be greater than or equal to 0");
        }

        if (value.PageSize > 10000)
        {
            return new ValidationException("Pagination.PageSize", "PageSize must be less than or equal to 10000");
        }

        return new Result<Unit>(Unit.Value);
    }
}
