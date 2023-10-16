using System;
using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Documents;

public class DeleteDocumentRequestValidator : Validator<DeleteDocumentRequest>
{
    private readonly ITemplateManager _templateManager;

    public DeleteDocumentRequestValidator(
        ITemplateManager templateManager
    )
    {
        _templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
        PropertyName.Push(nameof(DeleteDocumentRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(DeleteDocumentRequest value)
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
