using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;

namespace Persistify.Server.Management.Types.Abstractions;

public interface ITypeManager
{
    ValueTask IndexAsync(int templateId, Document document);
    ValueTask DeleteAsync(int templateId, long documentId);
    ValueTask InitializeForTemplate(int templateId);
    ValueTask RemoveForTemplate(int templateId);
}

public interface ITypeManager<in TQuery, THit> : ITypeManager
    where TQuery : ITypeManagerQuery
    where THit : ITypeManagerHit
{
    ValueTask<IEnumerable<THit>> SearchAsync(int templateId, TQuery query);
}
