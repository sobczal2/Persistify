using System.Threading.Tasks;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.FieldValues;

public class BinaryFieldValueDtoValidator : Validator<BinaryFieldValueDto>
{
    public BinaryFieldValueDtoValidator()
    {
        PropertyName.Push(nameof(BinaryFieldValueDto));
    }

    public override ValueTask<Result> ValidateNotNullAsync(BinaryFieldValueDto value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyName.Push(nameof(BinaryFieldValueDto.FieldName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(DocumentErrorMessages.NameEmpty)
            );
        }

        if (value.FieldName.Length > 64)
        {
            PropertyName.Push(nameof(BinaryFieldValueDto.FieldName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(DocumentErrorMessages.NameTooLong)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
