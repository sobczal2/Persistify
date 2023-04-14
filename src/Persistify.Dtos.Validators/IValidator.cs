using System.Collections.Generic;
using Persistify.Dtos.Response.Shared;

namespace Persistify.Dtos.Validators;

public interface IValidator<in T>
{
    IEnumerable<ValidationErrorDto> Validate(T dto);
}
