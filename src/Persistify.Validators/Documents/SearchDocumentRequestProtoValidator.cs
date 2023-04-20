using FluentValidation;
using Persistify.Protos;
using Persistify.Validators.Common;

namespace Persistify.Validators.Documents;

public class SearchDocumentRequestProtoValidator : AbstractValidator<SearchDocumentsRequestProto>
{
    public SearchDocumentRequestProtoValidator(IValidator<PaginationRequestProto> paginationRequestValidator)
    {
        RuleFor(x => x.Query)
            .NotNull()
            .WithErrorCode(DocumentErrorCodes.QueryMissing);

        RuleFor(x => x.PaginationRequest)
            .NotEmpty()
            .WithErrorCode(CommonErrorCodes.PaginationRequestEmpty)
            .SetValidator(paginationRequestValidator);
    }
}