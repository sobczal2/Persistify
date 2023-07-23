using Persistify.Domain.Documents;
using Persistify.Pipelines.Common;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;

namespace Persistify.Pipelines.Documents.GetDocument;

public class GetDocumentPipelineContext : IPipelineContext<GetDocumentRequest, GetDocumentResponse>
{
    public GetDocumentRequest Request { get; set; }
    public GetDocumentResponse? Response { get; set; }
    public Document? Document { get; set; }

    public GetDocumentPipelineContext(GetDocumentRequest request)
    {
        Request = request;
    }
}
