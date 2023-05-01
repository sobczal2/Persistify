using FluentValidation;
using Persistify.Protos;
using Persistify.Validators.Common;
using Persistify.Validators.Types;

namespace Persistify.Validators.Documents;

public class ComplexSearchDocumentsRequestProtoValidator : AbstractValidator<ComplexSearchDocumentsRequestProto>
{
    public ComplexSearchDocumentsRequestProtoValidator(
        IValidator<PaginationRequestProto> paginationRequestValidator
        )
    {
        RuleFor(x => x.TypeName)
            .NotEmpty()
            .WithErrorCode(TypeErrorCodes.NameEmpty);
        
        RuleFor(x => x.PaginationRequest)
            .NotEmpty()
            .WithErrorCode(CommonErrorCodes.PaginationRequestEmpty)
            .SetValidator(paginationRequestValidator);
    }
}