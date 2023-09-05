using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Templates;

public class GetTemplateRequestValidator : IValidator<GetTemplateRequest>
{
    public GetTemplateRequestValidator()
    {
        ErrorPrefix = "GetTemplateRequest";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(GetTemplateRequest value)
    {
        if (value.TemplateId <= 0)
        {
            return new ValidationException($"{ErrorPrefix}.TemplateId", "TemplateId must be greater than or equal to 0");
        }

        return Result.Ok;
    }
}
