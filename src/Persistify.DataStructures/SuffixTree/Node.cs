using System.Collections.Generic;

namespace Persistify.DataStructures.SuffixTree;

public class Node
{
    public Dictionary<char, Node> Children { get; set; }
    public Node? Parent { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public int Index { get; set; }

    public Node(int start, int end, Node? parent = null)
    {
        Start = start;
        End = end;
        Parent = parent;
        Children = new Dictionary<char, Node>();
    }
}