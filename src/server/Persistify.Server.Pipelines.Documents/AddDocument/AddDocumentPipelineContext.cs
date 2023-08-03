using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Documents.AddDocument;

public class AddDocumentPipelineContext : IPipelineContext<AddDocumentRequest, AddDocumentResponse>
{
    public AddDocumentPipelineContext(AddDocumentRequest request)
    {
        Request = request;
    }

    public int? DocumentId { get; set; }
    public AddDocumentRequest Request { get; set; }
    public AddDocumentResponse? Response { get; set; }
}
