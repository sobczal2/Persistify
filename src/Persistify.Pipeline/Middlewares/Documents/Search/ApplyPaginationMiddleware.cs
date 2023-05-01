using System;
using System.Threading.Tasks;
using Persistify.Helpers;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.Mutation)]
public class ApplyPaginationMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto
    , SearchDocumentsResponseProto>
{
    public Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        var indexes = context.Indexes ?? throw new InternalPipelineError();
        Array.Sort(indexes, (a, b) => a.Id.CompareTo(b.Id));
        context.Pagination = new PaginationResponseProto
        {
            TotalItems = indexes.Length,
            TotalPages = MathI.Ceiling(indexes.Length / (double)context.Request.PaginationRequest.PageSize),
            PageNumber = context.Request.PaginationRequest.PageNumber,
            PageSize = context.Request.PaginationRequest.PageSize
        };

        var startIndex = (context.Request.PaginationRequest.PageNumber - 1) *
                         context.Request.PaginationRequest.PageSize;
        var endIndex = startIndex + context.Request.PaginationRequest.PageSize;

        if (endIndex >= context.Indexes.Length)
        {
            endIndex = context.Indexes.Length;
        }

        context.Indexes = indexes[startIndex..endIndex];

        return Task.CompletedTask;
    }
}