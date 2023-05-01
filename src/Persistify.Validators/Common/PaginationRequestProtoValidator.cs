using FluentValidation;
using Persistify.Protos;

namespace Persistify.Validators.Common;

public class PaginationRequestProtoValidator : AbstractValidator<PaginationRequestProto>
{
    public PaginationRequestProtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(CommonErrorCodes.PageNumberInvalid);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(CommonErrorCodes.PageSizeInvalid);
    }
}