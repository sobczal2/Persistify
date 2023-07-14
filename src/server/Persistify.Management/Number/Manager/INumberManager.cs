using System.Collections.Generic;
using Persistify.Management.Common.Abstracts;
using Persistify.Management.Number.Search;
using Persistify.Management.Score;

namespace Persistify.Management.Number.Manager;

public interface INumberManager : IAddManager, IDeleteManager
{
    IEnumerable<NumberSearchHit> Search(string templateName, NumberQuery query,
        IScoreCalculator? scoreCalculator = null);
}
