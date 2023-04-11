using System.Collections.Generic;
using Persistify.Dtos.Request.Object;
using Persistify.Dtos.Response.Shared;

namespace Persistify.Dtos.Validators.Request.Object;

public class IndexObjectRequestDtoValidator : IValidator<IndexObjectRequestDto>
{
    public IEnumerable<ValidationErrorDto> Validate(IndexObjectRequestDto dto)
    {
        var errors = new List<ValidationErrorDto>();

        if (!Regexes.TypeName.IsMatch(dto.TypeName))
            errors.Add(new ValidationErrorDto
            {
                Field = nameof(dto.TypeName),
                Message = "Type name must be a valid C# type name"
            });

        return errors;
    }
}