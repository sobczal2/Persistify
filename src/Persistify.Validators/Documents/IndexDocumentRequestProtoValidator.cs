using System.Collections.Generic;
using Persistify.Protos;
using Persistify.Validators.Common;
using Persistify.Validators.Core;

namespace Persistify.Validators.Documents;

public class IndexDocumentRequestProtoValidator : IValidator<IndexDocumentRequestProto>
{
    public ValidationFailure[] Validate(IndexDocumentRequestProto instance)
    {
        var failures = new List<ValidationFailure>(1);

        if (string.IsNullOrEmpty(instance.TypeName)) failures.Add(ValidationFailures.TypeNameEmpty);

        return failures.ToArray();
    }
}