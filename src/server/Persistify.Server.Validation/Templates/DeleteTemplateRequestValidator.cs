using System;
using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Templates;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Templates;

public class DeleteTemplateRequestValidator : Validator<DeleteTemplateRequest>
{
    private readonly ITemplateManager _templateManager;

    public DeleteTemplateRequestValidator(
        ITemplateManager templateManager
    )
    {
        _templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
        PropertyName.Push(nameof(DeleteTemplateRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(DeleteTemplateRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(DeleteTemplateRequest.TemplateName));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(DeleteTemplateRequest.TemplateName));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (!_templateManager.Exists(value.TemplateName))
        {
            PropertyName.Push(nameof(DeleteTemplateRequest.TemplateName));
            return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.TemplateNotFound));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
