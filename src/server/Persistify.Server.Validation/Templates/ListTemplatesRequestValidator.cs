﻿using Persistify.Requests.Shared;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Templates;

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
        if (paginationResult.Failure)
        {
            return paginationResult;
        }

        return Result.Ok;
    }
}
