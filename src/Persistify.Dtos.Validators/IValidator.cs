using System.Collections.Generic;
using Persistify.Dtos.Response.Shared;

namespace Persistify.Dtos.Validators;

public interface IValidator<T>
{
    IEnumerable<ValidationErrorDto> Validate(T dto);
}