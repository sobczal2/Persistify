using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Templates;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Templates;

public class ExistsTemplateRequestValidator : Validator<ExistsTemplateRequest>
{
    public ExistsTemplateRequestValidator()
    {
        PropertyName.Push(nameof(ExistsTemplateRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(
        ExistsTemplateRequest value
    )
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(DeleteTemplateRequest.TemplateName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueNull)
            );
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(DeleteTemplateRequest.TemplateName));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueTooLong)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
