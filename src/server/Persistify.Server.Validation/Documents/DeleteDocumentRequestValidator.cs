using Persistify.Requests.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Documents;

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
            return new ValidationException($"{ErrorPrefix}.TemplateId",
                "TemplateId must be greater than or equal to 0");
        }

        if (value.DocumentId <= 0)
        {
            return new ValidationException($"{ErrorPrefix}.DocumentId",
                "DocumentId must be greater than or equal to 0");
        }

        return Result.Ok;
    }
}
