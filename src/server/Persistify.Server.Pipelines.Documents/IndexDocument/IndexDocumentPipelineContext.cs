using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Documents.IndexDocument;

public class IndexDocumentPipelineContext : IPipelineContext<IndexDocumentRequest, IndexDocumentResponse>
{
    public IndexDocumentRequest Request { get; set; }
    public IndexDocumentResponse? Response { get; set; }
    public long? DocumentId { get; set; }

    public IndexDocumentPipelineContext(IndexDocumentRequest request)
    {
        Request = request;
    }
}
