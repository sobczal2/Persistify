using System;
using Persistify.Requests.Shared;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
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

    public override Result ValidateNotNull(ListTemplatesRequest value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.Pagination == null)
        {
            PropertyName.Push(nameof(ListTemplatesRequest.Pagination));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        PropertyName.Push(nameof(ListTemplatesRequest.Pagination));
        var paginationResult = _paginationValidator.Validate(value.Pagination);
        PropertyName.Pop();
        if (paginationResult.Failure)
        {
            return paginationResult;
        }

        return Result.Ok;
    }
}
