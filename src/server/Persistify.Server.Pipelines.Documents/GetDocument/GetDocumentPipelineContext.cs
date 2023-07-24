using Persistify.Domain.Documents;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Documents.GetDocument;

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
