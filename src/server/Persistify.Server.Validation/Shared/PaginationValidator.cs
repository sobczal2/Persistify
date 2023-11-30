using System.Threading.Tasks;
using Persistify.Dtos.Common;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Shared;

public class PaginationValidator : Validator<PaginationDto>
{
    public PaginationValidator()
    {
        PropertyName.Push(nameof(PaginationDto));
    }

    public override ValueTask<Result> ValidateNotNullAsync(
        PaginationDto value
    )
    {
        if (value.PageNumber < 0)
        {
            PropertyName.Push(nameof(PaginationDto.PageNumber));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.PageNumberLessThanZero)
            );
        }

        if (value.PageSize <= 0)
        {
            PropertyName.Push(nameof(PaginationDto.PageSize));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.PageSizeLessThanOrEqualToZero)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
