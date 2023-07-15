using Persistify.Pipelines.Common;
using Persistify.Pipelines.Shared.Interfaces;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;
using Persistify.Protos.Documents.Shared;

namespace Persistify.Pipelines.Document.SearchDocument;

public class SearchDocumentsContext : IPipelineContext<SearchDocumentsRequest, SearchDocumentsResponse>, IWithTemplate
{
    public SearchDocumentsRequest Request { get; set; }
    public SearchDocumentsResponse? Response { get; set; }
    public Protos.Templates.Shared.Template? Template { get; set; }
    public string TemplateName { get; set; }
    public DocumentWithId[]? Documents { get; set; }
    public long? TotalCount { get; set; }

    public SearchDocumentsContext(SearchDocumentsRequest request, string templateName)
    {
        Request = request;
        TemplateName = templateName;
    }
}
