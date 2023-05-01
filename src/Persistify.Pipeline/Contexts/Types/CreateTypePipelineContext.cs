using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Contexts.Types;

public class CreateTypePipelineContext : PipelineContextBase<CreateTypeRequestProto, CreateTypeResponseProto>
{
    public CreateTypePipelineContext(CreateTypeRequestProto request) : base(request)
    {
    }
}