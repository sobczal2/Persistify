using Persistify.Indexes.Common;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Protos;
using Persistify.Tokens;

namespace Persistify.Pipeline.Contexts.Documents;

public class
    SearchDocumentsPipelineContext : PipelineContextBase<SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    public SearchDocumentsPipelineContext(SearchDocumentsRequestProto request) : base(request)
    {
    }
    public Token<string>[]? Tokens { get; set; }
    public Index[]? Indexes { get; set; }
    public PaginationResponseProto? Pagination { get; set; }
}