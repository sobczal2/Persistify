using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Documents;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.DocumentStore)]
public class FetchDocumentsFromDocumentStoreMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    private readonly IDocumentStore _documentStore;

    public FetchDocumentsFromDocumentStoreMiddleware(
        IDocumentStore documentStore
        )
    {
        _documentStore = documentStore;
    }

    public async Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        var documents = new List<DocumentProto>();
        var documentIds = context.DocumentIds ?? throw new InternalPipelineException();

        foreach (var documentId in documentIds)
        {
            documents.Add(new DocumentProto()
            {
                Id = documentId,
                Data = await _documentStore.GetAsync(documentId)
            });
        }

        context.SetResponse(new SearchDocumentsResponseProto()
        {
            Documents = { documents },
            PaginationResponse = context.PaginationResponse
        });
    }
}