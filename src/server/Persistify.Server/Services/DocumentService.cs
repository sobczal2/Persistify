﻿using System.Threading.Tasks;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class DocumentService : IDocumentService
{
    public ValueTask<IndexDocumentResponse> CreateDocumentAsync(IndexDocumentRequest request, CallContext context)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CallContext context)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request, CallContext context)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<DeleteDocumentResponse> DeleteDocumentAsync(DeleteDocumentRequest request, CallContext context)
    {
        throw new System.NotImplementedException();
    }
}
