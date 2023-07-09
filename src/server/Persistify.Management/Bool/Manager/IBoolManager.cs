using System.Collections.Generic;
using Persistify.Management.Bool.Search;
using Persistify.Management.Score;
using Persistify.Protos.Documents.Shared;
using BoolQuery = Persistify.Management.Bool.Search.BoolQuery;

namespace Persistify.Management.Bool.Manager;

public interface IBoolManager
{
    void Add(string templateName, Document document, long documentId);
    IEnumerable<BoolSearchHit> Search(string templateName, BoolQuery query, IScoreCalculator? scoreCalculator = null);
    void Delete(string templateName, long documentId);
}
