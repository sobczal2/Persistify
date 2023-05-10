using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Contexts.Documents;

public class
    RemoveDocumentPipelineContext : PipelineContextBase<RemoveDocumentRequestProto, RemoveDocumentResponseProto>
{
    public RemoveDocumentPipelineContext(RemoveDocumentRequestProto request) : base(request)
    {
    }
}