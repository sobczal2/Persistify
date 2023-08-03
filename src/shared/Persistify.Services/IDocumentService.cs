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
    ValueTask<AddDocumentResponse> AddDocumentAsync(AddDocumentRequest request);

    [OperationContract]
    ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request);

    [OperationContract]
    ValueTask<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request);

    [OperationContract]
    ValueTask<DeleteDocumentResponse> DeleteDocumentAsync(DeleteDocumentRequest request);
}
