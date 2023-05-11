using System.Collections.Generic;
using Persistify.Protos;
using Persistify.Validators.Common;
using Persistify.Validators.Core;

namespace Persistify.Validators.Documents;

public class SearchQueryValidator : IValidator<SearchQueryProto>
{
    private readonly IValidator<SearchBooleanQueryProto> _booleanQueryValidator;
    private readonly IValidator<SearchNumberQueryProto> _numberQueryValidator;
    private readonly IValidator<SearchTextQueryProto> _textQueryValidator;

    public SearchQueryValidator(
        IValidator<SearchNumberQueryProto> numberQueryValidator,
        IValidator<SearchTextQueryProto> textQueryValidator,
        IValidator<SearchBooleanQueryProto> booleanQueryValidator)
    {
        _numberQueryValidator = numberQueryValidator;
        _textQueryValidator = textQueryValidator;
        _booleanQueryValidator = booleanQueryValidator;
    }

    public ValidationFailure[] Validate(SearchQueryProto instance)
    {
        var failures = new List<ValidationFailure>();

        switch (instance.QueryCase)
        {
            case SearchQueryProto.QueryOneofCase.OrOperator:
                if (instance.OrOperator != null)
                    failures.AddRange(ValidateOr(instance.OrOperator));
                break;
            case SearchQueryProto.QueryOneofCase.AndOperator:
                if (instance.AndOperator != null)
                    failures.AddRange(ValidateAnd(instance.AndOperator));
                break;
            case SearchQueryProto.QueryOneofCase.NumberQuery:
                if (instance.NumberQuery != null)
                    failures.AddRange(_numberQueryValidator.Validate(instance.NumberQuery));
                break;
            case SearchQueryProto.QueryOneofCase.TextQuery:
                if (instance.TextQuery != null)
                    failures.AddRange(_textQueryValidator.Validate(instance.TextQuery));
                break;
            case SearchQueryProto.QueryOneofCase.BooleanQuery:
                if (instance.BooleanQuery != null)
                    failures.AddRange(_booleanQueryValidator.Validate(instance.BooleanQuery));
                break;
            case SearchQueryProto.QueryOneofCase.None:
                failures.Add(ValidationFailures.SearchQueryEmpty);
                break;
        }

        return failures.ToArray();
    }

    private ValidationFailure[] ValidateOr(SearchOrOperatorProto instance)
    {
        var failures = new List<ValidationFailure>();

        foreach (var query in instance.Queries)
            failures.AddRange(Validate(query));

        return failures.ToArray();
    }

    private ValidationFailure[] ValidateAnd(SearchAndOperatorProto instance)
    {
        var failures = new List<ValidationFailure>();

        foreach (var query in instance.Queries)
            failures.AddRange(Validate(query));

        return failures.ToArray();
    }
}