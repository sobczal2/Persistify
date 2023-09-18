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
    ValueTask<CreateDocumentResponse> CreateDocumentAsync(CreateDocumentRequest request, CallContext callContext);

    [OperationContract]
    ValueTask<GetDocumentResponse> GetDocumentAsync(GetDocumentRequest request, CallContext callContext);

    [OperationContract]
    ValueTask<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request, CallContext callContext);

    [OperationContract]
    ValueTask<DeleteDocumentResponse> DeleteDocumentAsync(DeleteDocumentRequest request, CallContext callContext);
}
