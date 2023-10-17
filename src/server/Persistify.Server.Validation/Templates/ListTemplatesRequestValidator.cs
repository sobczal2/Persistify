using System;
using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Shared;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Templates;

public class ListTemplatesRequestValidator : Validator<ListTemplatesRequest>
{
    private readonly IValidator<Pagination> _paginationValidator;

    public ListTemplatesRequestValidator(IValidator<Pagination> paginationValidator)
    {
        _paginationValidator = paginationValidator ?? throw new ArgumentNullException(nameof(paginationValidator));
        _paginationValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(ListTemplatesRequest));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(ListTemplatesRequest value)
    {
        PropertyName.Push(nameof(ListTemplatesRequest.Pagination));
        var paginationResult = await _paginationValidator.ValidateAsync(value.Pagination);
        PropertyName.Pop();
        if (paginationResult.Failure)
        {
            return paginationResult;
        }

        return Result.Ok;
    }
}
