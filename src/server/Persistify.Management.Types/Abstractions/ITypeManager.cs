using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;

namespace Persistify.Management.Types.Abstractions;

public interface ITypeManager
{
    ValueTask IndexAsync(Document document);
    ValueTask DeleteAsync(long documentId);
}

public interface ITypeManager<TQuery, THit> : ITypeManager
    where TQuery : ITypeManagerQuery
    where THit : ITypeManagerHit
{
    ValueTask<IEnumerable<THit>> SearchAsync(TQuery query);
}
