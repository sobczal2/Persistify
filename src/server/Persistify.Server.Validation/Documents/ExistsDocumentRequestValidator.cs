using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Documents;

public class ExistsDocumentRequestValidator : Validator<ExistsDocumentRequest>
{
    private readonly ITemplateManager _templateManager;

    public ExistsDocumentRequestValidator(
        ITemplateManager templateManager
        )
    {
        _templateManager = templateManager;
        PropertyName.Push(nameof(ExistsDocumentRequest));
    }
    public override ValueTask<Result> ValidateNotNullAsync(ExistsDocumentRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(DeleteDocumentRequest.TemplateName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(DeleteDocumentRequest.TemplateName));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (!_templateManager.Exists(value.TemplateName))
        {
            PropertyName.Push(nameof(DeleteDocumentRequest.TemplateName));
            return ValueTask.FromResult<Result>(DynamicValidationException(DocumentErrorMessages.TemplateNotFound));
        }

        if (value.DocumentId <= 0)
        {
            PropertyName.Push(nameof(DeleteDocumentRequest.DocumentId));
            return ValueTask.FromResult<Result>(StaticValidationException(DocumentErrorMessages.InvalidDocumentId));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
