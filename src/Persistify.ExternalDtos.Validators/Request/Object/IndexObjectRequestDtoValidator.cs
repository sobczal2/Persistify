using System.Collections.Generic;
using Persistify.ExternalDtos.Request.Object;
using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.ExternalDtos.Validators.Request.Object;

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