using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Persistify.Server.Fts.Tokens;

public class Token
{
    public Token(string value, int count, List<int> positions, float score, char[] alphabet)
    {
        Alphabet = alphabet;
        Value = value;
        Count = count;
        Positions = positions;
        Score = score;

        if (!CheckIfValueIsInAlphabet())
        {
            throw new ArgumentException("Either value is not in given alphabet or alphabet is not sorted",
                nameof(Alphabet));
        }

        AlphabetIndexMap = new List<int>(Value.Length);
        for (var i = 0; i < Value.Length; i++)
        {
            var index = Array.BinarySearch(Alphabet, Value[i]);
            if (index >= 0)
            {
                AlphabetIndexMap.Add(index);
            }
            else
            {
                throw new ArgumentException("Either value is not in given alphabet or alphabet is not sorted",
                    nameof(Alphabet));
            }
        }
    }

    public Token(string value, int position, float score, char[] alphabet) : this(value, 1, new List<int> {position}, score, alphabet)
    {
    }

    public char[] Alphabet { get; }
    public string Value { get; set; }
    public float Score { get; }
    public int Count { get; set; }
    public List<int> Positions { get; }
    public List<int> AlphabetIndexMap { get; private set; }

    private bool CheckIfValueIsInAlphabet()
    {
        return Value.All(x => Array.BinarySearch(Alphabet, x) >= 0);
    }
}
