using System.Collections.Generic;
using Persistify.Protos;
using Persistify.Validators.Core;

namespace Persistify.Validators.Documents;

public class RemoveDocumentRequestProtoValidator : IValidator<RemoveDocumentRequestProto>
{
    public ValidationFailure[] Validate(RemoveDocumentRequestProto instance)
    {
        var failures = new List<ValidationFailure>(1);

        if (string.IsNullOrEmpty(instance.TypeName))
        {
            failures.Add(ValidationFailures.TypeNameEmpty);
        }
        
        if (instance.DocumentId <= 0)
        {
            failures.Add(ValidationFailures.IdInvalid);
        }

        return failures.ToArray();
    }
}