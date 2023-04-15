using System;
using System.Collections.Generic;
using Persistify.DataStructures.MultiTargetTries.Exceptions;

namespace Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie;

public class MultiTargetTrie<TItem> : IMultiTargetTrie<TItem>
    where TItem : notnull
{
    private readonly byte _alphabetSize;
    private readonly MultiTargetTrieNode<TItem> _root;

    public MultiTargetTrie(byte alphabetSize)
    {
        _alphabetSize = alphabetSize;
        _root = new MultiTargetTrieNode<TItem>(alphabetSize);
    }

    public void Add(string key, TItem item, ISingleTargetMapper mapper)
    {
        if (mapper.AlphabetSize > _alphabetSize)
            throw new AlphabetTooLargeException();
        var keySpan = key.AsSpan();
        var currentNode = _root;

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < keySpan.Length; i++)
        {
            var mappedValue = mapper.MapToIndex(keySpan[i]);
            currentNode = currentNode.GetOrAddChild(mappedValue);
        }

        currentNode.AddItem(item);
    }

    public IEnumerable<TItem> Search(string query, IMultiTargetMapper mapper)
    {
        if (mapper.AlphabetSize > _alphabetSize)
            throw new AlphabetTooLargeException();

        var keySpan = query.AsSpan();
        var queue = new Queue<MultiTargetTrieNode<TItem>>();
        var resultList = new List<TItem>();

        queue.Enqueue(_root);

        for (var i = 0; i < keySpan.Length; i++)
        {
            var mappedValues = mapper.MapToIndexes(keySpan[i]);

            // Process all nodes at the current level
            var currentLevelNodeCount = queue.Count;
            while (currentLevelNodeCount-- > 0)
            {
                var currentNode = queue.Dequeue();
                foreach (var mappedValue in mappedValues)
                {
                    var child = currentNode.GetChild(mappedValue);
                    if (child != null)
                        queue.Enqueue(child);
                }
            }
        }

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            resultList.AddRange(currentNode.Items);
        }

        return resultList;
    }

    public bool Contains(string key, IMultiTargetMapper mapper)
    {
        if (mapper.AlphabetSize > _alphabetSize)
            throw new AlphabetTooLargeException();

        var keySpan = key.AsSpan();
        var queue = new Queue<MultiTargetTrieNode<TItem>>();

        queue.Enqueue(_root);

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < keySpan.Length; i++)
        {
            var mappedValues = mapper.MapToIndexes(keySpan[i]);
            for (var j = 0; j < queue.Count; j++)
            {
                var currentNode = queue.Dequeue();
                foreach (var mappedValue in mappedValues)
                {
                    var child = currentNode.GetChild(mappedValue);
                    if (child != null)
                        queue.Enqueue(child);
                }
            }
        }

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            if (currentNode.Items.Count > 0)
                return true;
        }

        return false;
    }

    public bool Remove(TItem item)
    {
        throw new NotImplementedException();
    }

    public MultiTargetTrieStats GetStats()
    {
        throw new NotImplementedException();
    }
}