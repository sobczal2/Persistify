using System.Threading.Tasks;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.FieldValues;

public class TextFieldValueDtoValidator : Validator<TextFieldValueDto>
{
    public TextFieldValueDtoValidator()
    {
        PropertyName.Push(nameof(TextFieldValueDto));
    }

    public override ValueTask<Result> ValidateNotNullAsync(TextFieldValueDto value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyName.Push(nameof(TextFieldValueDto.FieldName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(DocumentErrorMessages.NameEmpty)
            );
        }

        if (value.FieldName.Length > 64)
        {
            PropertyName.Push(nameof(TextFieldValueDto.FieldName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(DocumentErrorMessages.NameTooLong)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
