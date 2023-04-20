using Newtonsoft.Json.Linq;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Protos;
using Persistify.Tokens;

namespace Persistify.Pipeline.Contexts.Documents;

public class IndexDocumentPipelineContext : PipelineContextBase<IndexDocumentRequestProto, IndexDocumentResponseProto>
{
    public IndexDocumentPipelineContext(IndexDocumentRequestProto request) : base(request)
    {
    }

    public TypeDefinitionProto? TypeDefinition { get; set; }
    public JObject? Data { get; set; }
    public long? DocumentId { get; set; }
    public Token<string>[]? TextTokens { get; set; }
    public Token<double>[]? NumberTokens { get; set; }
    public Token<bool>[]? BooleanTokens { get; set; }
}