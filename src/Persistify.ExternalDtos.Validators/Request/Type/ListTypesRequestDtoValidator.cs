using System.Collections.Generic;
using Persistify.ExternalDtos.Common.Pagination;
using Persistify.ExternalDtos.Request.Type;
using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.ExternalDtos.Validators.Request.Type;

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