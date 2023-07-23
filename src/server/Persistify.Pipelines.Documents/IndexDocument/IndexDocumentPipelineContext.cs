using Persistify.Pipelines.Common;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;

namespace Persistify.Pipelines.Documents.IndexDocument;

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
