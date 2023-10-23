using System.Threading.Tasks;
using Persistify.Dtos.Common;
using Persistify.Helpers.Results;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.PresetAnalyzers;

public class ListPresetAnalyzersRequestValidator : Validator<ListPresetAnalyzersRequest>
{
    private readonly IValidator<PaginationDto> _paginationValidator;

    public ListPresetAnalyzersRequestValidator(IValidator<PaginationDto> paginationValidator)
    {
        _paginationValidator = paginationValidator;
        PropertyName.Push(nameof(ListPresetAnalyzersRequest));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(ListPresetAnalyzersRequest value)
    {
        PropertyName.Push(nameof(ListPresetAnalyzersRequest.PaginationDto));
        var paginationResult = await _paginationValidator.ValidateAsync(value.PaginationDto);
        PropertyName.Pop();
        if (paginationResult.Failure)
        {
            return paginationResult;
        }

        return Result.Ok;
    }
}
