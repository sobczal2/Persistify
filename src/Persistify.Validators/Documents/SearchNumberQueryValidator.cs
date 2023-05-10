using System.Collections.Generic;
using Persistify.Protos;
using Persistify.Validators.Core;

namespace Persistify.Validators.Documents;

public class SearchNumberQueryValidator : IValidator<SearchNumberQueryProto>
{
    public ValidationFailure[] Validate(SearchNumberQueryProto instance)
    {
        var failures = new List<ValidationFailure>();

        if (string.IsNullOrEmpty(instance.Path)) failures.Add(ValidationFailures.FieldPathEmpty);

        if (instance.Min > instance.Max) failures.Add(ValidationFailures.NumberRangeInvalid);

        return failures.ToArray();
    }
}