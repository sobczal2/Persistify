using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Middlewares.Documents.ComplexSearch;

[PipelineStep(PipelineStepType.DynamicValidation)]
public class ValidateQueryAgainstTypeMiddleware : IPipelineMiddleware<ComplexSearchDocumentsPipelineContext,
    ComplexSearchDocumentsRequestProto, ComplexSearchDocumentsResponseProto>
{
    public async Task InvokeAsync(ComplexSearchDocumentsPipelineContext context)
    {
    }
}