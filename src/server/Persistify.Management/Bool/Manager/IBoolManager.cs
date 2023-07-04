using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Management.Bool.Search;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using BoolQuery = Persistify.Management.Bool.Search.BoolQuery;

namespace Persistify.Management.Bool.Manager;

public interface IBoolManager
{
    ValueTask AddAsync(string templateName, Document document, ulong documentId, CancellationToken cancellationToken = default);
    ValueTask<ICollection<BoolSearchHit>> SearchAsync(string templateName, BoolQuery query, IScoreCalculator? scoreCalculator = null, CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(string templateName, ulong documentId, CancellationToken cancellationToken = default);
}
