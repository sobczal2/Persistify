using System;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Helpers;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.Mutation)]
public class ApplyPaginationMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    public async Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        var paginationRequest = context.Request.PaginationRequest ?? throw new InternalPipelineException();
        var documentIds = context.DocumentIds ?? throw new InternalPipelineException();
        var totalCount = documentIds.Length;
        
        Array.Sort(documentIds);

        context.DocumentIds = documentIds
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToArray();
        
        context.PaginationResponse = new PaginationResponseProto
        {
            PageNumber = paginationRequest.PageNumber,
            PageSize = paginationRequest.PageSize,
            TotalItems = totalCount,
            TotalPages = MathI.Ceiling(totalCount / (double)paginationRequest.PageSize)
        };
        
        await Task.CompletedTask;
    }
}