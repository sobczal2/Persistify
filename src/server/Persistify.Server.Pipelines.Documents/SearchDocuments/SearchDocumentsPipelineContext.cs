using System.Collections.Generic;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Management.Shared;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Contexts;

namespace Persistify.Server.Pipelines.Documents.SearchDocuments;

public class SearchDocumentsPipelineContext : IPipelineContext<SearchDocumentsRequest, SearchDocumentsResponse>, IContextWithTemplate
{
    public SearchDocumentsPipelineContext(SearchDocumentsRequest request, int templateId)
    {
        Request = request;
        TemplateId = templateId;
    }

    public SearchDocumentsRequest Request { get; set; }
    public SearchDocumentsResponse? Response { get; set; }
    public int TemplateId { get; set; }
    public Template? Template { get; set; }
    public List<DocumentScore>? DocumentScores { get; set; }
    public List<long>? DocumentIds { get; set; }
    public int? TotalCount { get; set; }
    public List<Document>? Documents { get; set; }
}
