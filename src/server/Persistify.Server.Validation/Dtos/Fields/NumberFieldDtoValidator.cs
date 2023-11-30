using System.Threading.Tasks;
using Persistify.Dtos.Templates.Fields;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.Fields;

public class NumberFieldDtoValidator : Validator<NumberFieldDto>
{
    public NumberFieldDtoValidator()
    {
        PropertyName.Push(nameof(NumberFieldDto));
    }

    public override ValueTask<Result> ValidateNotNullAsync(
        NumberFieldDto value
    )
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyName.Push(nameof(NumberFieldDto.Name));
            return ValueTask.FromResult<Result>(
                StaticValidationException(TemplateErrorMessages.NameEmpty)
            );
        }

        if (value.Name.Length > 64)
        {
            PropertyName.Push(nameof(NumberFieldDto.Name));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueTooLong)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
