using System.Collections.Generic;
using Persistify.Protos;
using Persistify.Validators.Core;

namespace Persistify.Validators.Documents;

public class SearchTextQueryValidator : IValidator<SearchTextQueryProto>
{
    public ValidationFailure[] Validate(SearchTextQueryProto instance)
    {
        var failures = new List<ValidationFailure>();

        if (string.IsNullOrEmpty(instance.Path)) failures.Add(ValidationFailures.FieldPathEmpty);

        foreach (var character in instance.Value)
        {
            if (!char.IsLetterOrDigit(character) && character != '?')
                failures.Add(ValidationFailures.FieldValueInvalidCharacter);
        }

        return failures.ToArray();
    }
}