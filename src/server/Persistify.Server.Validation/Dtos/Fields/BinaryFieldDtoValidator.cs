using System.Threading.Tasks;
using Persistify.Dtos.Templates.Fields;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.Fields;

public class BinaryFieldDtoValidator : Validator<BinaryFieldDto>
{
    public BinaryFieldDtoValidator()
    {
        PropertyName.Push(nameof(BinaryFieldDto));
    }

    public override ValueTask<Result> ValidateNotNullAsync(BinaryFieldDto value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyName.Push(nameof(BoolFieldDto.Name));
            return ValueTask.FromResult<Result>(
                StaticValidationException(TemplateErrorMessages.NameEmpty)
            );
        }

        if (value.Name.Length > 64)
        {
            PropertyName.Push(nameof(BoolFieldDto.Name));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueTooLong)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
