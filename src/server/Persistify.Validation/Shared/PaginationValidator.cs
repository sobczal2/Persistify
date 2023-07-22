using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Shared;
using Persistify.Validation.Common;

namespace Persistify.Validation.Shared;

public class PaginationValidator : IValidator<Pagination>
{
    public PaginationValidator()
    {
        ErrorPrefix = "Pagination";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(Pagination value)
    {
        if (value.PageNumber < 0)
        {
            return new ValidationException($"{ErrorPrefix}.PageNumber", "PageNumber must be greater than 0");
        }

        if (value.PageSize <= 0)
        {
            return new ValidationException($"{ErrorPrefix}.PageSize", "PageSize must be greater than 0");
        }

        return Result.Success;
    }
}
