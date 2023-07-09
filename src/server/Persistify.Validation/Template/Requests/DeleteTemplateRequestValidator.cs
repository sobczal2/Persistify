using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Templates.Requests;
using Persistify.Validation.Common;

namespace Persistify.Validation.Template.Requests;

public class DeleteTemplateRequestValidator : IValidator<DeleteTemplateRequest>
{
    public Result<Unit> Validate(DeleteTemplateRequest value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException("DeleteTemplateRequest", "Request is null");
        }

        if (string.IsNullOrEmpty(value.TemplateName))
        {
            return new ValidationException("DeleteTemplateRequest", "TemplateName is null or empty");
        }

        return new Result<Unit>(Unit.Value);
    }
}
