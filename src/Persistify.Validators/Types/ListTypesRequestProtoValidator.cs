using FluentValidation;
using Persistify.Protos;
using Persistify.Validators.Common;

namespace Persistify.Validators.Types;

public class ListTypesRequestProtoValidator : AbstractValidator<ListTypesRequestProto>
{
    public ListTypesRequestProtoValidator()
    {
        RuleFor(x => x.PaginationRequest)
            .NotEmpty()
            .WithErrorCode(CommonErrorCodes.PaginationRequestEmpty);
    }
}