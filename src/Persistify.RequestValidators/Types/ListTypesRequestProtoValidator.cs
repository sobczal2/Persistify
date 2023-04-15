using FluentValidation;
using Persistify.Protos;
using Persistify.RequestValidators.Common;

namespace Persistify.RequestValidators.Types;

public class ListTypesRequestProtoValidator : AbstractValidator<ListTypesRequestProto>
{
    public ListTypesRequestProtoValidator()
    {
        RuleFor(x => x.PaginationRequest)
            .NotEmpty()
            .WithErrorCode(CommonErrorCodes.PaginationRequestEmpty);
    }
}