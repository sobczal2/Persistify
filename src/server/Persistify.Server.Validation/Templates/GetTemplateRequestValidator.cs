﻿using System.Threading.Tasks;
using Persistify.Requests.Templates;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Templates;

public class GetTemplateRequestValidator : Validator<GetTemplateRequest>
{
    private readonly ITemplateManager _templateManager;

    public GetTemplateRequestValidator(
        ITemplateManager templateManager
    )
    {
        _templateManager = templateManager;
        PropertyName.Push(nameof(GetTemplateRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(GetTemplateRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(GetTemplateRequest.TemplateName));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.TemplateName.Length > 64)
        {
            PropertyName.Push(nameof(GetTemplateRequest.TemplateName));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (!_templateManager.Exists(value.TemplateName))
        {
            PropertyName.Push(nameof(GetTemplateRequest.TemplateName));
            return ValueTask.FromResult<Result>(ValidationException(TemplateErrorMessages.TemplateNotFound));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
