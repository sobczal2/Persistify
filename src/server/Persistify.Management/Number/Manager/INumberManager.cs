using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Management.Number.Search;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using NumberQuery = Persistify.Management.Number.Search.NumberQuery;

namespace Persistify.Management.Number.Manager;

public interface INumberManager
{
    ValueTask AddAsync(string templateName, Document document, ulong documentId, CancellationToken cancellationToken = default);
    ValueTask<ICollection<NumberSearchHit>> SearchAsync(string templateName, NumberQuery query, IScoreCalculator? scoreCalculator = null, CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(string templateName, ulong documentId, CancellationToken cancellationToken = default);
}
