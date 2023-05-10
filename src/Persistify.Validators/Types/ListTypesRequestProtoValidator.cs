using Persistify.Protos;
using Persistify.Validators.Core;

namespace Persistify.Validators.Types;

public class ListTypesRequestProtoValidator : IValidator<ListTypesRequestProto>
{
    private readonly IValidator<PaginationRequestProto> _paginationRequestProtoValidator;

    public ListTypesRequestProtoValidator(IValidator<PaginationRequestProto> paginationRequestProtoValidator)
    {
        _paginationRequestProtoValidator = paginationRequestProtoValidator;
    }

    public ValidationFailure[] Validate(ListTypesRequestProto instance)
    {
        return instance.PaginationRequest is null
            ? new[] { ValidationFailures.PaginationEmpty }
            : _paginationRequestProtoValidator.Validate(instance.PaginationRequest);
    }
}