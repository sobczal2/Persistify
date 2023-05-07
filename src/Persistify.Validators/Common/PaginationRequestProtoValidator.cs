using System.Collections.Generic;
using Persistify.Protos;
using Persistify.Validators.Core;

namespace Persistify.Validators.Common;

public class PaginationRequestProtoValidator : IValidator<PaginationRequestProto>
{
    public ValidationFailure[] Validate(PaginationRequestProto instance)
    {
        var failures = new List<ValidationFailure>(2);

        if (instance.PageNumber < 1)
        {
            failures.Add(ValidationFailures.PageNumberInvalid);
        }

        if (instance.PageSize < 1)
        {
            failures.Add(ValidationFailures.PageSizeInvalid);
        }

        return failures.ToArray();
    }
}