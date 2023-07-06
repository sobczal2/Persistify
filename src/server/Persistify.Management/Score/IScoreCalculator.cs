namespace Persistify.Management.Score;

public interface IScoreCalculator
{
    float Calculate(float termFrequencySum);
}
