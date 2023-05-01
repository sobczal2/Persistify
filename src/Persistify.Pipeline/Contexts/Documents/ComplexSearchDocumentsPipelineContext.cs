using System.Collections.Generic;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Contexts.Documents;

public class ComplexSearchDocumentsPipelineContext : PipelineContextBase<ComplexSearchDocumentsRequestProto, ComplexSearchDocumentsResponseProto>
{
    public ComplexSearchDocumentsPipelineContext(ComplexSearchDocumentsRequestProto request) : base(request)
    {
    }

    public TypeDefinitionProto? TypeDefinition { get; set; }
    public IEnumerable<long> DocumentIds { get; set; }

}