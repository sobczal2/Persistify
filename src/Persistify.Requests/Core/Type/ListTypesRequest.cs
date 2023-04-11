using Persistify.Protos;
using Persistify.Requests.Common;

namespace Persistify.Requests.Core.Type;

public class ListTypesRequest : CoreRequest<ListTypesRequestProto, ListTypesResponseProto>
{
    public ListTypesRequest(ListTypesRequestProto request) : base(request)
    {
    }
}