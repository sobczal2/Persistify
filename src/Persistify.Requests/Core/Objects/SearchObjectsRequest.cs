using Persistify.Protos;
using Persistify.Requests.Common;

namespace Persistify.Requests.Core.Objects;

public class SearchObjectsRequest : CoreRequest<SearchObjectsRequestProto, SearchObjectsResponseProto>
{
    public SearchObjectsRequest(SearchObjectsRequestProto request) : base(request)
    {
    }
}