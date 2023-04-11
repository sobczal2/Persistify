using System.Collections.Generic;
using Persistify.Dtos.Common.Types;
using Persistify.Dtos.Response.Shared;

namespace Persistify.Dtos.Validators.Common.Types;

public class TypeDefinitionDtoValidator : IValidator<TypeDefinitionDto>
{
    private readonly IValidator<FieldDefinitionDto> _fieldDefinitionDtoValidator;

    public TypeDefinitionDtoValidator(IValidator<FieldDefinitionDto> fieldDefinitionDtoValidator)
    {
        _fieldDefinitionDtoValidator = fieldDefinitionDtoValidator;
    }

    public IEnumerable<ValidationErrorDto> Validate(TypeDefinitionDto dto)
    {
        var errors = new List<ValidationErrorDto>();

        if (!Regexes.TypeName.IsMatch(dto.TypeName))
            errors.Add(new ValidationErrorDto
            {
                Field = nameof(dto.TypeName),
                Message = "Type name must be a valid C# type name"
            });

        if (dto.Fields == null || dto.Fields.Length == 0)
            errors.Add(new ValidationErrorDto
            {
                Field = nameof(dto.Fields),
                Message = "Type must have at least one field"
            });

        if (dto.Fields != null)
            foreach (var field in dto.Fields)
                errors.AddRange(_fieldDefinitionDtoValidator.Validate(field));

        var fieldNames = new HashSet<string>();

        if (dto.Fields != null)
            foreach (var field in dto.Fields)
                if (fieldNames.Contains(field.Path))
                    errors.Add(new ValidationErrorDto
                    {
                        Field = nameof(dto.Fields),
                        Message = $"Duplicate field path: {field.Path}"
                    });
                else
                    fieldNames.Add(field.Path);

        if (dto.Fields != null)
            if (fieldNames.Contains(dto.IdFieldPath))
                errors.Add(new ValidationErrorDto
                {
                    Field = nameof(dto.IdFieldPath),
                    Message = "Primary key must not be a field path"
                });

        return errors;
    }
}