using Persistify.Protos;
using Persistify.Requests.Common;

namespace Persistify.Requests.Core.Type;

public class CreateTypeRequest : CoreRequest<CreateTypeRequestProto, CreateTypeResponseProto>
{
    public CreateTypeRequest(CreateTypeRequestProto request) : base(request)
    {
    }
}