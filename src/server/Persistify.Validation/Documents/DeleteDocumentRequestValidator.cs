using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Validation.Common;

namespace Persistify.Validation.Documents;

public class DeleteDocumentRequestValidator : IValidator<DeleteDocumentRequest>
{
    public DeleteDocumentRequestValidator()
    {
        ErrorPrefix = "DeleteDocumentRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(DeleteDocumentRequest value)
    {
        if (value.TemplateId <= 0)
        {
            return new ValidationException($"{ErrorPrefix}.TemplateId", "TemplateId must be greater than 0");
        }

        if (value.DocumentId <= 0)
        {
            return new ValidationException($"{ErrorPrefix}.DocumentId", "DocumentId must be greater than 0");
        }

        return Result.Success;
    }
}
