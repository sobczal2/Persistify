using Persistify.Pipelines.Common;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;
using Persistify.Protos.Documents.Shared;

namespace Persistify.Pipelines.Document.GetDocument;

public class GetDocumentContext : IPipelineContext<GetDocumentRequest, GetDocumentResponse>
{
    public GetDocumentContext(GetDocumentRequest request)
    {
        Request = request;
    }

    public DocumentWithId? Document { get; set; }
    public GetDocumentRequest Request { get; set; }
    public GetDocumentResponse? Response { get; set; }
}
