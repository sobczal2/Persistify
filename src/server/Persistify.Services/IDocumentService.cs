using System.ServiceModel;
using System.Threading.Tasks;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using ProtoBuf.Grpc;

namespace Persistify.Services;

[ServiceContract]
public interface IDocumentService
{
    [OperationContract]
    ValueTask<IndexDocumentResponse> IndexDocumentAsync(IndexDocumentRequest request, CallContext context);

    [OperationContract]
    ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CallContext context);

    [OperationContract]
    ValueTask<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request, CallContext context);

    [OperationContract]
    ValueTask<DeleteDocumentResponse> DeleteDocumentAsync(DeleteDocumentRequest request, CallContext context);
}
