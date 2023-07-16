using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Document.AddDocuments.Stages;
using Persistify.Pipelines.Exceptions;
using Persistify.Pipelines.Shared.Stages;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;

namespace Persistify.Pipelines.Document.AddDocuments;

public class AddDocumentsPipeline : Pipeline<AddDocumentsContext, AddDocumentsRequest, AddDocumentsResponse>
{
    private readonly FetchTemplateFromManagerStage<AddDocumentsContext, AddDocumentsRequest, AddDocumentsResponse>
        _fetchTemplateFromManagerStage;

    private readonly StoreDocumentsStage _storeDocumentsStage;

    private readonly ValidateDocumentsAgainstTemplateStage _validateDocumentsAgainstTemplateStage;
    private readonly ValidationStage<AddDocumentsContext, AddDocumentsRequest, AddDocumentsResponse> _validationStage;

    public AddDocumentsPipeline(
        ILogger<AddDocumentsPipeline> logger,
        ValidationStage<AddDocumentsContext, AddDocumentsRequest, AddDocumentsResponse> validationStage,
        FetchTemplateFromManagerStage<AddDocumentsContext, AddDocumentsRequest, AddDocumentsResponse>
            fetchTemplateFromManagerStage,
        ValidateDocumentsAgainstTemplateStage validateDocumentsAgainstTemplateStage,
        StoreDocumentsStage storeDocumentsStage
    ) : base(
        logger
    )
    {
        _validationStage = validationStage;
        _fetchTemplateFromManagerStage = fetchTemplateFromManagerStage;
        _validateDocumentsAgainstTemplateStage = validateDocumentsAgainstTemplateStage;
        _storeDocumentsStage = storeDocumentsStage;
    }

    protected override PipelineStage<AddDocumentsContext, AddDocumentsRequest, AddDocumentsResponse>[] PipelineStages
        => new PipelineStage<AddDocumentsContext, AddDocumentsRequest, AddDocumentsResponse>[]
        {
            _validationStage, _fetchTemplateFromManagerStage, _validateDocumentsAgainstTemplateStage,
            _storeDocumentsStage
        };

    protected override AddDocumentsContext CreateContext(AddDocumentsRequest request)
    {
        return new AddDocumentsContext(request, request.TemplateName);
    }

    protected override AddDocumentsResponse CreateResponse(AddDocumentsContext context)
    {
        return new AddDocumentsResponse(context.DocumentIds ?? throw new PipelineException());
    }
}
