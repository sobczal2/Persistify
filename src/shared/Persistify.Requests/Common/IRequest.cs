using Persistify.Responses.Common;

namespace Persistify.Requests.Common;

public interface IRequest<TResponse>
    where TResponse : IResponse
{
}
