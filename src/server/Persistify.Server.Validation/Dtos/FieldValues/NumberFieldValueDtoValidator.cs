using System.Threading.Tasks;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.FieldValues;

public class NumberFieldValueDtoValidator : Validator<NumberFieldValueDto>
{
    public NumberFieldValueDtoValidator()
    {
        PropertyName.Push(nameof(NumberFieldValueDto));
    }

    public override ValueTask<Result> ValidateNotNullAsync(
        NumberFieldValueDto value
    )
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyName.Push(nameof(NumberFieldValueDto.FieldName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(DocumentErrorMessages.NameEmpty)
            );
        }

        if (value.FieldName.Length > 64)
        {
            PropertyName.Push(nameof(NumberFieldValueDto.FieldName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(DocumentErrorMessages.NameTooLong)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
