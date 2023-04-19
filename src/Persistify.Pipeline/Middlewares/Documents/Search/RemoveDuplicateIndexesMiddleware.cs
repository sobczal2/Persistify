using System.Linq;
using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.Mutation)]
public class RemoveDuplicateIndexesMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    public Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        var indexes = context.Indexes ?? throw new InternalPipelineError();

        context.Indexes = indexes.DistinctBy(x => x.Id).ToArray();
        
        return Task.CompletedTask;
    }
}