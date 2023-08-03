using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Documents;

public class GetDocumentRequestValidator : IValidator<GetDocumentRequest>
{
    public GetDocumentRequestValidator()
    {
        ErrorPrefix = "GetDocumentRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(GetDocumentRequest value)
    {
        if (value.TemplateId < 0)
        {
            return new ValidationException($"{ErrorPrefix}.TemplateId", "TemplateId must be greater than or equal to 0");
        }

        if (value.DocumentId < 0)
        {
            return new ValidationException($"{ErrorPrefix}.DocumentId", "DocumentId must be greater than or equal to 0");
        }

        return Result.Success;
    }
}
