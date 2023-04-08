using System.Collections.Generic;
using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.ExternalDtos.Validators;

public interface IValidator<T>
{
    IEnumerable<ValidationErrorDto> Validate(T dto);
}