using System.Collections.Generic;
using Persistify.Protos;
using Persistify.Validators.Core;

namespace Persistify.Validators.Documents;

public class SearchBooleanQueryValidator : IValidator<SearchBooleanQueryProto>
{
    public ValidationFailure[] Validate(SearchBooleanQueryProto instance)
    {
        var failures = new List<ValidationFailure>();

        if (string.IsNullOrEmpty(instance.Path))
        {
            failures.Add(ValidationFailures.FieldPathEmpty);
        }

        return failures.ToArray();
    }
}