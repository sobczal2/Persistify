namespace Persistify.DataStructures.Tries;

public record TrieStats
(
    long Nodes,
    long Leaves,
    long MaxDepth,
    long AverageDepth,
    long MaxBranchingFactor,
    long AverageBranchingFactor
);