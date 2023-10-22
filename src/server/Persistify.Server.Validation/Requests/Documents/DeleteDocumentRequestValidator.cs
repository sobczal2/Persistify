using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Documents;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Documents;

public class DeleteDocumentRequestValidator : Validator<DeleteDocumentRequest>
{
    public DeleteDocumentRequestValidator(
    )
    {
        PropertyName.Push(nameof(DeleteDocumentRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(DeleteDocumentRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(DeleteDocumentRequest.TemplateName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(DeleteDocumentRequest.TemplateName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (value.DocumentId <= 0)
        {
            PropertyName.Push(nameof(DeleteDocumentRequest.DocumentId));
            return ValueTask.FromResult<Result>(StaticValidationException(DocumentErrorMessages.InvalidDocumentId));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
