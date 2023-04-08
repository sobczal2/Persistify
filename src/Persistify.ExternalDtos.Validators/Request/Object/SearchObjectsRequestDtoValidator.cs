using System.Collections.Generic;
using Persistify.ExternalDtos.Common.Pagination;
using Persistify.ExternalDtos.Request.Object;
using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.ExternalDtos.Validators.Request.Object;

public class SearchObjectsRequestDtoValidator : IValidator<SearchObjectsRequestDto>
{
    private readonly IValidator<PaginationRequestDto> _paginationRequestDtoValidator;

    public SearchObjectsRequestDtoValidator(IValidator<PaginationRequestDto> paginationRequestDtoValidator)
    {
        _paginationRequestDtoValidator = paginationRequestDtoValidator;
    }

    public IEnumerable<ValidationErrorDto> Validate(SearchObjectsRequestDto dto)
    {
        var errors = new List<ValidationErrorDto>();

        if (!Regexes.TypeName.IsMatch(dto.TypeName))
            errors.Add(new ValidationErrorDto
            {
                Field = nameof(dto.TypeName),
                Message = "Type name must be a valid C# type name"
            });

        if (!Regexes.Query.IsMatch(dto.Query))
            errors.Add(new ValidationErrorDto
            {
                Field = nameof(dto.Query),
                Message = "Query must be a valid Lucene query"
            });

        errors.AddRange(_paginationRequestDtoValidator.Validate(dto.PaginationRequest));

        foreach (var path in dto.FieldPaths)
            if (!Regexes.JsonPath.IsMatch(path))
                errors.Add(new ValidationErrorDto
                {
                    Field = nameof(dto.FieldPaths),
                    Message = "Field path must be a valid JSON path"
                });

        return errors;
    }
}