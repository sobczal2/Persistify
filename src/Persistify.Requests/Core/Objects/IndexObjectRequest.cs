using Persistify.Protos;
using Persistify.Requests.Common;

namespace Persistify.Requests.Core.Objects;

public class IndexObjectRequest : CoreRequest<IndexObjectRequestProto, IndexObjectResponseProto>
{
    public IndexObjectRequest(IndexObjectRequestProto request) : base(request)
    {
    }
}