using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.DynamicValidation)]
public class ValidateQueryAgainstTypeMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext,
    SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    public async Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
    }
}