using System.Collections.Generic;
using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Request.Type;
using Persistify.Dtos.Response.Shared;

namespace Persistify.Dtos.Validators.Request.Type;

public class ListTypesRequestDtoValidator : IValidator<ListTypesRequestDto>
{
    private readonly IValidator<PaginationRequestDto> _paginationRequestDtoValidator;

    public ListTypesRequestDtoValidator(IValidator<PaginationRequestDto> paginationRequestDtoValidator)
    {
        _paginationRequestDtoValidator = paginationRequestDtoValidator;
    }

    public IEnumerable<ValidationErrorDto> Validate(ListTypesRequestDto dto)
    {
        var errors = new List<ValidationErrorDto>();

        errors.AddRange(_paginationRequestDtoValidator.Validate(dto.PaginationRequest));

        return errors;
    }
}