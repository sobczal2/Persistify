using System.Collections.Generic;
using Persistify.Management.Number.Search;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using Persistify.Protos.Documents.Shared;
using NumberQuery = Persistify.Management.Number.Search.NumberQuery;

namespace Persistify.Management.Number.Manager;

public interface INumberManager
{
    void Add(string templateName, Document document, long documentId);

    IEnumerable<NumberSearchHit> Search(string templateName, NumberQuery query,
        IScoreCalculator? scoreCalculator = null);

    void Delete(string templateName, long documentId);
}
