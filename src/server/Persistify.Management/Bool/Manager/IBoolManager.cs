using System.Collections.Generic;
using Persistify.Management.Bool.Search;
using Persistify.Management.Common.Abstracts;
using Persistify.Management.Score;

namespace Persistify.Management.Bool.Manager;

public interface IBoolManager : IAddManager, IDeleteManager
{
    IEnumerable<BoolSearchHit> Search(string templateName, BoolQuery query, IScoreCalculator? scoreCalculator = null);
}
