using System.ServiceModel;
using System.Threading.Tasks;
using Persistify.Requests.Users;
using Persistify.Responses.Users;
using ProtoBuf.Grpc;

namespace Persistify.Services;

[ServiceContract]
public interface IUserService
{
    [OperationContract]
    ValueTask<CreateUserResponse> CreateUserAsync(CreateUserRequest request, CallContext context);

    [OperationContract]
    ValueTask<GetUserResponse> GetUserAsync(GetUserRequest request, CallContext context);
}
