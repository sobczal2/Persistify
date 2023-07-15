using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Templates.Requests;
using Persistify.Validation.Common;

namespace Persistify.Validation.Template.Requests;

public class DeleteTemplateRequestValidator : IValidator<DeleteTemplateRequest>
{
    public DeleteTemplateRequestValidator()
    {
        ErrorPrefix = "DeleteTemplateRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(DeleteTemplateRequest value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException(ErrorPrefix, "Request is null");
        }

        if (string.IsNullOrEmpty(value.TemplateName))
        {
            return new ValidationException(ErrorPrefix, "TemplateName is null or empty");
        }

        return Result.Success;
    }
}
