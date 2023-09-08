using Persistify.Requests.Shared;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Templates;

public class ListTemplatesRequestValidator : Validator<ListTemplatesRequest>
{
    private readonly IValidator<Pagination> _paginationValidator;

    public ListTemplatesRequestValidator(IValidator<Pagination> paginationValidator)
    {
        _paginationValidator = paginationValidator;
        _paginationValidator.PropertyNames = PropertyNames;
        PropertyNames.Push(nameof(ListTemplatesRequest));
    }

    public override Result Validate(ListTemplatesRequest value)
    {
        PropertyNames.Push(nameof(ListTemplatesRequest.Pagination));
        var paginationResult = _paginationValidator.Validate(value.Pagination);
        PropertyNames.Pop();
        if (paginationResult.Failure)
        {
            return paginationResult;
        }

        return Result.Ok;
    }
}
