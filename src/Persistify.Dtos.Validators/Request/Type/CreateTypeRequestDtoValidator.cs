using System.Collections.Generic;
using Persistify.Dtos.Common.Types;
using Persistify.Dtos.Request.Type;
using Persistify.Dtos.Response.Shared;

namespace Persistify.Dtos.Validators.Request.Type;

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

        errors.AddRange(_typeDefinitionDtoValidator.Validate(dto.TypeDefinition));

        return errors;
    }
}