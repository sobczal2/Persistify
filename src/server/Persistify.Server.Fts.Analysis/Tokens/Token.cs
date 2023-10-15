﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Persistify.Domain.Fts;

public class Token : IEnumerable<int>
{
    public char[] Alphabet { get; }
    public string Value { get; set; }
    public float Score { get; }
    public int Count { get; set; }
    public List<int> Positions { get; }
    public Token(string value, int count, List<int> positions, float score, char[] alphabet)
    {
        Alphabet = alphabet;
        Value = value;
        Count = count;
        Positions = positions;
        Score = score;
    }

    public Token(string value, int position, float score, char[] alphabet)
    {
        Alphabet = alphabet;
        Value = value;
        Score = score;
        Count = 1;
        Positions = new List<int>(1) { position };
    }

    // alphabet has to be sorted
    private IEnumerable<int> CreateValueAlphabetIndexMap()
    {
        for (var i = 0; i < Value.Length; i++)
        {
            var index = Array.BinarySearch(Alphabet, Value[i]);
            if (index >= 0)
            {
                yield return index;
            }
            else
            {
                throw new ArgumentException("Either value is not in given alphabet or alphabet is not sorted",
                    nameof(Alphabet));
            }
        }
    }

    public IEnumerator<int> GetEnumerator()
    {
        return CreateValueAlphabetIndexMap().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)CreateValueAlphabetIndexMap()).GetEnumerator();
    }
}