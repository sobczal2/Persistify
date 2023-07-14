using System.Collections.Generic;
using Persistify.Management.Common.Abstracts;
using Persistify.Management.Fts.Search;
using Persistify.Management.Score;

namespace Persistify.Management.Fts.Manager;

public interface IFtsManager : IAddManager, IDeleteManager
{
    IEnumerable<FtsSearchHit> Search(string templateName, FtsQuery query, IScoreCalculator? scoreCalculator = null);
}
