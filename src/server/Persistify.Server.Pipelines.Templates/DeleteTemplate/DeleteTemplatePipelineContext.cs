﻿using Persistify.Domain.Templates;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Templates.DeleteTemplate;

public class DeleteTemplatePipelineContext : IPipelineContext<DeleteTemplateRequest, DeleteTemplateResponse>
{
    public DeleteTemplatePipelineContext(DeleteTemplateRequest request)
    {
        Request = request;
    }

    public Template? Template { get; set; }
    public DeleteTemplateRequest Request { get; set; }
    public DeleteTemplateResponse? Response { get; set; }
}
