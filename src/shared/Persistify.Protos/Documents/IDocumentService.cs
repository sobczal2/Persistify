using System.ServiceModel;
using System.Threading.Tasks;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;
using ProtoBuf.Grpc;

namespace Persistify.Protos.Documents;

[ServiceContract]
public interface IDocumentService
{
    [OperationContract]
    ValueTask<AddDocumentsResponse> Add(AddDocumentsRequest request, CallContext context);

    [OperationContract]
    ValueTask<GetDocumentsResponse> Get(GetDocumentsRequest request, CallContext context);

    [OperationContract]
    ValueTask<SearchDocumentsResponse> Search(SearchDocumentsRequest request, CallContext context);

    [OperationContract]
    ValueTask<DeleteDocumentsResponse> Delete(DeleteDocumentsRequest request, CallContext context);
}
