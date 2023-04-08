using System.Collections.Generic;
using Persistify.ExternalDtos.Common.Types;
using Persistify.ExternalDtos.Request.Type;
using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.ExternalDtos.Validators.Request.Type;

public class CreateTypeRequestDtoValidator : IValidator<CreateTypeRequestDto>
{
    private readonly IValidator<TypeDefinitionDto> _typeDefinitionDtoValidator;

    public CreateTypeRequestDtoValidator(IValidator<TypeDefinitionDto> typeDefinitionDtoValidator)
    {
        _typeDefinitionDtoValidator = typeDefinitionDtoValidator;
    }

    public IEnumerable<ValidationErrorDto> Validate(CreateTypeRequestDto dto)
    {
        var errors = new List<ValidationErrorDto>();

        if (dto.TypeDefinition == null)
            errors.Add(new ValidationErrorDto
            {
                Field = nameof(dto.TypeDefinition),
                Message = "Type definition must be provided"
            });
        else
            errors.AddRange(_typeDefinitionDtoValidator.Validate(dto.TypeDefinition));

        return errors;
    }
}