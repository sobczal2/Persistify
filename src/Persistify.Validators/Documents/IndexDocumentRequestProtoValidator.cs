using FluentValidation;
using Persistify.Protos;

namespace Persistify.Validators.Documents;

public class IndexDocumentRequestProtoValidator : AbstractValidator<IndexDocumentRequestProto>
{
    public IndexDocumentRequestProtoValidator()
    {
        RuleFor(x => x.TypeName)
            .NotEmpty();
    }
}