using System.Collections.Generic;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Contexts.Documents;

public class SearchDocumentsPipelineContext : PipelineContextBase<SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    public SearchDocumentsPipelineContext(SearchDocumentsRequestProto request) : base(request)
    {
    }

    public TypeDefinitionProto? TypeDefinition { get; set; }
    public long[]? DocumentIds { get; set; }
    public PaginationResponseProto? PaginationResponse { get; set; }

}