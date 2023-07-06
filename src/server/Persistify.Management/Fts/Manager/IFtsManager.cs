using System.Collections.Generic;
using Persistify.Management.Fts.Search;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using FtsQuery = Persistify.Management.Fts.Search.FtsQuery;

namespace Persistify.Management.Fts.Manager;

public interface IFtsManager
{
    void Add(string templateName, Document document, ulong documentId);
    IEnumerable<FtsSearchHit> Search(string templateName, FtsQuery query, IScoreCalculator? scoreCalculator = null);
    void Delete(string templateName, ulong documentId);
}
