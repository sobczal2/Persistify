using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.Mutation)]
public class FilterIndexesByFieldsMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    public Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        var searchPaths = context.Request.SearchPaths;
        if(searchPaths.Count == 0)
            return Task.CompletedTask;
        
        var indexes = context.Indexes ?? throw new InternalPipelineError();
        var filteredIndexes = new List<Indexes.Common.Index>();

        foreach (var index in indexes)
        {
            if(searchPaths.Contains(index.Path))
                filteredIndexes.Add(index);
        }
        
        context.Indexes = filteredIndexes.ToArray();
        
        return Task.CompletedTask;
    }
}