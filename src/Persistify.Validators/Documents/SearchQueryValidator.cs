using System.Collections.Generic;
using Persistify.Protos;
using Persistify.Validators.Core;

namespace Persistify.Validators.Documents;

public class SearchQueryValidator : IValidator<SearchQuery>
{
    private readonly IValidator<SearchNumberQueryProto> _numberQueryValidator;
    private readonly IValidator<SearchTextQueryProto> _textQueryValidator;
    private readonly IValidator<SearchBooleanQueryProto> _booleanQueryValidator;

    public SearchQueryValidator(
        IValidator<SearchNumberQueryProto> numberQueryValidator,
        IValidator<SearchTextQueryProto> textQueryValidator,
        IValidator<SearchBooleanQueryProto> booleanQueryValidator)
    {
        _numberQueryValidator = numberQueryValidator;
        _textQueryValidator = textQueryValidator;
        _booleanQueryValidator = booleanQueryValidator;
    }

    public ValidationFailure[] Validate(SearchQuery instance)
    {
        var failures = new List<ValidationFailure>();

        switch (instance.QueryCase)
        {
            case SearchQuery.QueryOneofCase.Or:
                if(instance.Or != null)
                    failures.AddRange(ValidateOr(instance.Or));
                break;
            case SearchQuery.QueryOneofCase.And:
                if(instance.And != null)
                    failures.AddRange(ValidateAnd(instance.And));
                break;
            case SearchQuery.QueryOneofCase.NumberQuery:
                if(instance.NumberQuery != null)
                    failures.AddRange(_numberQueryValidator.Validate(instance.NumberQuery));
                break;
            case SearchQuery.QueryOneofCase.TextQuery:
                if(instance.TextQuery != null)
                    failures.AddRange(_textQueryValidator.Validate(instance.TextQuery));
                break;
            case SearchQuery.QueryOneofCase.BooleanQuery:
                if(instance.BooleanQuery != null)
                    failures.AddRange(_booleanQueryValidator.Validate(instance.BooleanQuery));
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