using Persistify.Pipelines.Common;
using Persistify.Pipelines.Shared.Interfaces;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;
using Persistify.Protos.Documents.Shared;

namespace Persistify.Pipelines.Document.GetDocument;

public class GetDocumentContext : IPipelineContext<GetDocumentRequest, GetDocumentResponse>
{
    public GetDocumentRequest Request { get; set; }
    public GetDocumentResponse? Response { get; set; }
    public DocumentWithId? Document { get; set; }

    public GetDocumentContext(GetDocumentRequest request)
    {
        Request = request;
    }

}
