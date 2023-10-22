using System;
using System.Threading.Tasks;
using Persistify.Dtos.Common;
using Persistify.Helpers.Results;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Templates;

public class ListTemplatesRequestValidator : Validator<ListTemplatesRequest>
{
    private readonly IValidator<PaginationDto> _paginationValidator;

    public ListTemplatesRequestValidator(IValidator<PaginationDto> paginationValidator)
    {
        _paginationValidator = paginationValidator ?? throw new ArgumentNullException(nameof(paginationValidator));
        _paginationValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(ListTemplatesRequest));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(ListTemplatesRequest value)
    {
        PropertyName.Push(nameof(ListTemplatesRequest.PaginationDto));
        var paginationResult = await _paginationValidator.ValidateAsync(value.PaginationDto);
        PropertyName.Pop();
        if (paginationResult.Failure)
        {
            return paginationResult;
        }

        return Result.Ok;
    }
}
