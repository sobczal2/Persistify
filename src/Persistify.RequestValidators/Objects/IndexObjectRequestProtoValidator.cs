using FluentValidation;
using Persistify.Protos;

namespace Persistify.RequestValidators.Objects;

public class IndexObjectRequestProtoValidator : AbstractValidator<IndexObjectRequestProto>
{
    public IndexObjectRequestProtoValidator()
    {
        RuleFor(x => x.TypeName)
            .NotEmpty();
    }
}