using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Templates;

public class DeleteTemplateRequestValidator : IValidator<DeleteTemplateRequest>
{
    public DeleteTemplateRequestValidator()
    {
        ErrorPrefix = "DeleteTemplateRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(DeleteTemplateRequest value)
    {
        if (value.TemplateId < 0)
        {
            return new ValidationException($"{ErrorPrefix}.TemplateId", "TemplateId must be greater than or equal to 0");
        }

        return Result.Success;
    }
}
