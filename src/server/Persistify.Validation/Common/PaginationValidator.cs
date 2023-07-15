using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Common;

namespace Persistify.Validation.Common;

public class PaginationValidator : IValidator<Pagination>
{
    public PaginationValidator()
    {
        ErrorPrefix = "Pagination";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(Pagination value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException(ErrorPrefix, "Pagination is null");
        }

        if (value.PageSize < 1)
        {
            return new ValidationException($"{ErrorPrefix}.PageSize", "PageSize must be greater than 0");
        }

        if (value.PageNumber < 0)
        {
            return new ValidationException($"{ErrorPrefix}.PageNumber",
                "PageNumber must be greater than or equal to 0");
        }

        if (value.PageSize > 10000)
        {
            return new ValidationException($"{ErrorPrefix}.PageSize", "PageSize must be less than or equal to 10000");
        }

        return Result.Success;
    }
}
