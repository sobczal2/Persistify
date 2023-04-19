using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Contexts.Objects;

public class IndexDocumentPipelineContext : PipelineContextBase<IndexObjectRequestProto, IndexObjectResponseProto>
{
    public IndexDocumentPipelineContext(IndexObjectRequestProto request) : base(request)
    {
    }

    public TypeDefinitionProto? TypeDefinition { get; set; }
    public long? DocumentId { get; set; }
}