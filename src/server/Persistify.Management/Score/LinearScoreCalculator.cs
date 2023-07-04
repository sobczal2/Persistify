using System;

namespace Persistify.Management.Score;

public class LinearScoreCalculator : IScoreCalculator
{
    private readonly int _multiplier;
    private readonly int _maxScore;

    public LinearScoreCalculator(int multiplier = 1, int maxScore = 100)
    {
        _multiplier = multiplier;
        _maxScore = maxScore;
    }
    public float Calculate(int count)
    {
        return Math.Min(count * _multiplier, _maxScore);
    }
}
