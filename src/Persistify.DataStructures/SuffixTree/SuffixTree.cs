using System.Collections.Generic;

namespace Persistify.DataStructures.SuffixTree;

public class SuffixTree
{
    public Node Root { get; set; }
    public string Text { get; set; }

    public SuffixTree(string text)
    {
        Text = text;
        Root = new Node(-1, -1);
        BuildSuffixTree();
    }
    
    private void BuildSuffixTree()
    {
        for (int i = 0; i < Text.Length; i++)
        {
            AddSuffix(i);
        }
    }
    
    private void AddSuffix(int index)
    {
        var currentNode = Root;
        var start = index;

        while (start < Text.Length)
        {
            if (currentNode.Children.ContainsKey(Text[start]))
            {
                currentNode = currentNode.Children[Text[start]];
                var suffixIndex = start;

                while (suffixIndex < Text.Length && currentNode.Start + (suffixIndex - start) <= currentNode.End)
                {
                    if (Text[suffixIndex] != Text[currentNode.Start + (suffixIndex - start)])
                    {
                        var splitEnd = currentNode.Start + (suffixIndex - start) - 1;
                        var splitNode = new Node(currentNode.Start, splitEnd, currentNode.Parent);
                        currentNode.Start = splitEnd + 1;
                        currentNode.Parent.Children[Text[splitNode.Start]] = splitNode;
                        splitNode.Children[Text[currentNode.Start]] = currentNode;
                        currentNode.Parent = splitNode;

                        currentNode = splitNode;
                        break;
                    }
                    suffixIndex++;
                }

                if (suffixIndex == Text.Length)
                {
                    currentNode.Index = index;
                    return;
                }
                else
                {
                    start = suffixIndex;
                }
            }
            else
            {
                Node newNode = new Node(start, Text.Length - 1, currentNode);
                newNode.Index = index;
                currentNode.Children[Text[start]] = newNode;
                return;
            }
        }
    }
    
    public List<int> Search(string pattern)
    {
        var result = new List<int>();
        var currentNode = Root;
        var patternIndex = 0;

        while (patternIndex < pattern.Length)
        {
            if (currentNode.Children.ContainsKey(pattern[patternIndex]))
            {
                currentNode = currentNode.Children[pattern[patternIndex]];
                var nodeIndex = currentNode.Start;

                while (patternIndex < pattern.Length && nodeIndex <= currentNode.End)
                {
                    if (pattern[patternIndex] != Text[nodeIndex])
                    {
                        return result;
                    }
                    patternIndex++;
                    nodeIndex++;
                }

                if (nodeIndex <= currentNode.End)
                {
                    return result;
                }
            }
            else
            {
                return result;
            }
        }
        
        CollectIndices(currentNode, result);
        return result;
    }

    private static void CollectIndices(Node node, ICollection<int> indices)
    {
        if (node.Index != -1)
        {
            indices.Add(node.Index);
        }

        foreach (var child in node.Children.Values)
        {
            CollectIndices(child, indices);
        }
    }
}