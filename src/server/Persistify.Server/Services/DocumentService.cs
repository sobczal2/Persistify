using System;
using System.Threading.Tasks;
using Persistify.Protos.Documents;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class DocumentService : IDocumentService
{
    public ValueTask<AddDocumentsResponse> Add(AddDocumentsRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }

    public ValueTask<GetDocumentsResponse> Get(GetDocumentsRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }

    public ValueTask<SearchDocumentsResponse> Search(SearchDocumentsRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }

    public ValueTask<DeleteDocumentsResponse> Delete(DeleteDocumentsRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }
}
