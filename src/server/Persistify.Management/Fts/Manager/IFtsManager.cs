using System.Collections.Generic;
using Persistify.Management.Fts.Search;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using Persistify.Protos.Documents.Shared;
using FtsQuery = Persistify.Management.Fts.Search.FtsQuery;

namespace Persistify.Management.Fts.Manager;

public interface IFtsManager
{
    void Add(string templateName, Document document, long documentId);
    IEnumerable<FtsSearchHit> Search(string templateName, FtsQuery query, IScoreCalculator? scoreCalculator = null);
    void Delete(string templateName, long documentId);
}
