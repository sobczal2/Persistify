using System.Collections.Generic;
using Persistify.ExternalDtos.Common.Types;
using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.ExternalDtos.Validators.Common.Types;

public class FieldDefinitionDtoValidator : IValidator<FieldDefinitionDto>
{
    public IEnumerable<ValidationErrorDto> Validate(FieldDefinitionDto dto)
    {
        var errors = new List<ValidationErrorDto>();

        if (Regexes.JsonPath.IsMatch(dto.Path) == false)
            errors.Add(new ValidationErrorDto
            {
                Field = nameof(dto.Path),
                Message = "Field path must be a valid JSON path"
            });

        return errors;
    }
}