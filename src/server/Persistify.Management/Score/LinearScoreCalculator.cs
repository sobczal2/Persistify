using System;

namespace Persistify.Management.Score;

public class LinearScoreCalculator : IScoreCalculator
{
    private readonly int _maxScore;
    private readonly int _multiplier;

    public LinearScoreCalculator(int multiplier = 1, int maxScore = 100)
    {
        _multiplier = multiplier;
        _maxScore = maxScore;
    }

    public float Calculate(float count)
    {
        return Math.Min(count * _multiplier, _maxScore);
    }
}
