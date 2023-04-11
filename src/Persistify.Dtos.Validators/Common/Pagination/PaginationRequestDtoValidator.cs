using System.Collections.Generic;
using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Response.Shared;

namespace Persistify.Dtos.Validators.Common.Pagination;

public class PaginationRequestDtoValidator : IValidator<PaginationRequestDto>
{
    public IEnumerable<ValidationErrorDto> Validate(PaginationRequestDto dto)
    {
        var errors = new List<ValidationErrorDto>();

        if (dto.PageNumber < 1)
            errors.Add(new ValidationErrorDto
            {
                Field = nameof(dto.PageNumber),
                Message = "Page number must be greater than 0"
            });

        if (dto.PageSize < 1)
            errors.Add(new ValidationErrorDto
            {
                Field = nameof(dto.PageSize),
                Message = "Page size must be greater than 0"
            });

        return errors;
    }
}