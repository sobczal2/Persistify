using Persistify.Pipelines.Common;
using Persistify.Pipelines.Shared.Interfaces;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;

namespace Persistify.Pipelines.Document.AddDocuments;

public class AddDocumentsContext : IPipelineContext<AddDocumentsRequest, AddDocumentsResponse>, IWithTemplate
{
    public long[]? DocumentIds { get; set; }
    public AddDocumentsRequest Request { get; set; }
    public AddDocumentsResponse? Response { get; set; }
    public Protos.Templates.Shared.Template? Template { get; set; }
    public string TemplateName { get; set; }

    public AddDocumentsContext(AddDocumentsRequest request, string templateName)
    {
        Request = request;
        TemplateName = templateName;
    }
}
