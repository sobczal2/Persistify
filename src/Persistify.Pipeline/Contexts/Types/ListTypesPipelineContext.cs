using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Contexts.Types;

public class ListTypesPipelineContext : PipelineContextBase<ListTypesRequestProto, ListTypesResponseProto>
{
    public ListTypesPipelineContext(ListTypesRequestProto request) : base(request)
    {
    }
}