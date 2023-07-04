using System.Collections.Generic;

namespace Persistify.Management.Score;

public interface IScoreCalculator
{
    float Calculate(int count);
}
