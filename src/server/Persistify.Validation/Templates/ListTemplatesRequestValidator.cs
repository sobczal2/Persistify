using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Shared;
using Persistify.Requests.Templates;
using Persistify.Validation.Common;

namespace Persistify.Validation.Templates;

public class ListTemplatesRequestValidator : IValidator<ListTemplatesRequest>
{
    private readonly IValidator<Pagination> _paginationValidator;

    public ListTemplatesRequestValidator(IValidator<Pagination> paginationValidator)
    {
        _paginationValidator = paginationValidator;
        ErrorPrefix = "ListTemplatesRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(ListTemplatesRequest value)
    {
        _paginationValidator.ErrorPrefix = $"{ErrorPrefix}.Pagination";
        var paginationResult = _paginationValidator.Validate(value.Pagination);
        if (paginationResult.IsFailure)
        {
            return paginationResult;
        }

        return Result.Success;
    }
}
