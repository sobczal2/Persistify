using System;
using System.Threading.Tasks;
using Persistify.Requests.Documents;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Documents;

public class GetDocumentRequestValidator : Validator<GetDocumentRequest>
{
    private readonly ITemplateManager _templateManager;

    public GetDocumentRequestValidator(
        ITemplateManager templateManager
    )
    {
        _templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
        PropertyName.Push(nameof(GetDocumentRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(GetDocumentRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(GetDocumentRequest.TemplateName));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(GetDocumentRequest.TemplateName));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (!_templateManager.Exists(value.TemplateName))
        {
            PropertyName.Push(nameof(GetDocumentRequest.TemplateName));
            return ValueTask.FromResult<Result>(ValidationException(DocumentErrorMessages.TemplateNotFound));
        }

        if (value.DocumentId <= 0)
        {
            PropertyName.Push(nameof(GetDocumentRequest.DocumentId));
            return ValueTask.FromResult<Result>(ValidationException(DocumentErrorMessages.InvalidDocumentId));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
