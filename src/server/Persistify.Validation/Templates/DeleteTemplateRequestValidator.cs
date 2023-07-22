using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Validation.Common;

namespace Persistify.Validation.Templates;

public class DeleteTemplateRequestValidator : IValidator<DeleteTemplateRequest>
{
    public DeleteTemplateRequestValidator()
    {
        ErrorPrefix = "DeleteTemplateRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(DeleteTemplateRequest value)
    {
        if (value.TemplateId <= 0)
        {
            return new ValidationException($"{ErrorPrefix}.TemplateId", "TemplateId must be greater than 0");
        }

        return Result.Success;
    }
}
