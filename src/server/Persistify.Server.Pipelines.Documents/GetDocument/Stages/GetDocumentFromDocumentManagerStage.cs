﻿using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Exceptions.Templates;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Documents.GetDocument.Stages;

public class
    GetDocumentFromDocumentManagerStage : PipelineStage<GetDocumentPipelineContext, GetDocumentRequest,
        GetDocumentResponse>
{
    public const string StageName = "GetDocumentFromDocumentManager";
    private readonly IDocumentManager _documentManager;

    public GetDocumentFromDocumentManagerStage(
        ILogger<GetDocumentFromDocumentManagerStage> logger,
        IDocumentManager documentManager
    ) : base(logger)
    {
        _documentManager = documentManager;
    }

    public override string Name => StageName;

    public override async ValueTask ProcessAsync(GetDocumentPipelineContext context)
    {
        try
        {
            context.Document = await _documentManager.GetAsync(context.Request.TemplateId, context.Request.DocumentId);
        }
        catch (TemplateNotFoundException)
        {
            throw new ValidationException("GetDocumentRequest.TemplateId", "Template not found");
        }

        if (context.Document == null)
        {
            throw new ValidationException("GetDocumentRequest.DocumentId", "Document not found");
        }
    }
}
