using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Contexts.Objects;

public class IndexObjectPipelineContext : PipelineContextBase<IndexObjectRequestProto, IndexObjectResponseProto>
{
    public IndexObjectPipelineContext(IndexObjectRequestProto request) : base(request)
    {
    }

    public TypeDefinitionProto? TypeDefinition { get; set; }
}