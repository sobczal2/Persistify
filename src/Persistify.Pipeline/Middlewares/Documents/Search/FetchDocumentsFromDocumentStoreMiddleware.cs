using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Documents;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.DocumentStore)]
public class FetchDocumentsFromDocumentStoreMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext,
    SearchDocumentsRequestProto, SearchDocumentsResponseProto>
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
        var indexes = context.Indexes ?? throw new InternalPipelineError();
        var documentProtos = new DocumentProto[indexes.Length];
        for (var i = 0; i < indexes.Length; i++)
        {
            documentProtos[i] = new DocumentProto()
            {
                Id = indexes[i].Id,
                Data = await _documentStore.GetAsync(indexes[i].Id)
            };
        }
        

        context.SetResponse(new SearchDocumentsResponseProto
        {
            PaginationResponse = context.Pagination,
            Documents = { documentProtos }
        });
    }
}