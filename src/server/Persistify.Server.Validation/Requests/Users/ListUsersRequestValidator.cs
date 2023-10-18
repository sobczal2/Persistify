using System.Threading.Tasks;
using Persistify.Dtos.Common;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Users;

public class ListUsersRequestValidator : Validator<ListUsersRequest>
{
    private readonly IValidator<PaginationDto> _paginationValidator;

    public ListUsersRequestValidator(
        IValidator<PaginationDto> paginationValidator
    )
    {
        _paginationValidator = paginationValidator;
        PropertyName.Push(nameof(ListUsersRequest));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(ListUsersRequest value)
    {
        PropertyName.Push(nameof(ListUsersRequest.Pagination));
        var paginationResult = await _paginationValidator.ValidateAsync(value.Pagination);
        PropertyName.Pop();
        if (paginationResult.Failure)
        {
            return paginationResult;
        }

        return Result.Ok;
    }
}
