using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Management.Fts.Search;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using FtsQuery = Persistify.Management.Fts.Search.FtsQuery;

namespace Persistify.Management.Fts.Manager;

public interface IFtsManager
{
    ValueTask AddAsync(string templateName, Document document, ulong documentId, CancellationToken cancellationToken = default);
    ValueTask<ICollection<FtsSearchHit>> SearchAsync(string templateName, FtsQuery query, IScoreCalculator? scoreCalculator = null, CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(string templateName, ulong documentId, CancellationToken cancellationToken = default);
}
