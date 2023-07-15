using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Documents.Requests;
using Persistify.Validation.Common;

namespace Persistify.Validation.Document.Requests;

public class AddDocumentRequestValidator : IValidator<AddDocumentsRequest>
{
    private readonly IValidator<Protos.Documents.Shared.Document> _documentValidator;

    public AddDocumentRequestValidator(IValidator<Protos.Documents.Shared.Document> documentValidator)
    {
        _documentValidator = documentValidator;
        ErrorPrefix = "AddDocumentsRequest";
    }

    public string ErrorPrefix { get; set; }


    public Result Validate(AddDocumentsRequest value)
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

        if (value.Documents.Length == 0)
        {
            return new ValidationException($"{ErrorPrefix}.Documents", "Documents is empty");
        }

        for (var i = 0; i < value.Documents.Length; i++)
        {
            _documentValidator.ErrorPrefix = $"{ErrorPrefix}.Documents[{i}]";

            var result = _documentValidator.Validate(value.Documents[i]);
            if (result.IsFailure)
            {
                return result;
            }
        }

        return Result.Success;
    }
}
