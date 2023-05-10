using System.Collections.Generic;
using Persistify.Protos;
using Persistify.Validators.Core;

namespace Persistify.Validators.Documents;

public class SearchDocumentsRequestProtoValidator : IValidator<SearchDocumentsRequestProto>
{
    private readonly IValidator<PaginationRequestProto> _paginationRequestValidator;
    private readonly IValidator<SearchQueryProto> _searchQueryValidator;

    public SearchDocumentsRequestProtoValidator(
        IValidator<SearchQueryProto> searchQueryValidator,
        IValidator<PaginationRequestProto> paginationRequestValidator
    )
    {
        _searchQueryValidator = searchQueryValidator;
        _paginationRequestValidator = paginationRequestValidator;
    }

    public ValidationFailure[] Validate(SearchDocumentsRequestProto instance)
    {
        var failures = new List<ValidationFailure>();

        if (string.IsNullOrEmpty(instance.TypeName)) failures.Add(ValidationFailures.TypeNameEmpty);

        if (instance.Query is null)
            failures.Add(ValidationFailures.QueryEmpty);
        else
            failures.AddRange(_searchQueryValidator.Validate(instance.Query));

        if (instance.PaginationRequest is null)
            failures.Add(ValidationFailures.PaginationEmpty);
        else
            failures.AddRange(_paginationRequestValidator.Validate(instance.PaginationRequest));


        return failures.ToArray();
    }
}