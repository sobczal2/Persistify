using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Common;
using Persistify.Protos.Templates.Requests;
using Persistify.Validation.Common;

namespace Persistify.Validation.Template.Requests;

public class ListTemplatesRequestValidator : IValidator<ListTemplatesRequest>
{
    private readonly IValidator<Pagination> _paginationValidator;

    public ListTemplatesRequestValidator(IValidator<Pagination> paginationValidator)
    {
        _paginationValidator = paginationValidator;
    }

    public Result<Unit> Validate(ListTemplatesRequest value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException("ListTemplatesRequest", "Request is null");
        }

        var result = _paginationValidator.Validate(value.Pagination);
        if (result.IsFailure)
        {
            return result;
        }

        return new Result<Unit>(Unit.Value);
    }
}
