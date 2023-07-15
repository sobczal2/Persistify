using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Documents.Requests;
using Persistify.Validation.Common;

namespace Persistify.Validation.Document.Requests;

public class GetDocumentRequestValidator : IValidator<GetDocumentRequest>
{
    public string ErrorPrefix { get; set; }

    public GetDocumentRequestValidator()
    {
        ErrorPrefix = "GetDocumentRequest";
    }
    public Result<Unit> Validate(GetDocumentRequest value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException(ErrorPrefix, "Request is null");
        }

        if (string.IsNullOrEmpty(value.TemplateName))
        {
            return new ValidationException($"{ErrorPrefix}.TemplateName", "TemplateName is required");
        }

        if (value.TemplateName.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.TemplateName",
                "TemplateName's length must be lower than or equal to 64 characters");
        }

        if (value.DocumentId <= 0)
        {
            return new ValidationException($"{ErrorPrefix}.DocumentId", "DocumentId is invalid");
        }

        return new Result<Unit>(Unit.Value);
    }
}
