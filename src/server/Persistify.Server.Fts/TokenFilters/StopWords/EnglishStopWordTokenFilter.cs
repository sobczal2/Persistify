using System.Collections.Generic;
using System.Text;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.TokenFilters;

public class EnglishStopWordTokenFilter : ITokenFilter
{
    private static readonly IReadOnlySet<string> StopWords = new HashSet<string>
    {
        "a",
        "about",
        "above",
        "after",
        "again",
        "against",
        "all",
        "am",
        "an",
        "and",
        "any",
        "are",
        "arent",
        "as",
        "at",
        "be",
        "because",
        "been",
        "before",
        "being",
        "below",
        "between",
        "both",
        "but",
        "by",
        "cant",
        "cannot",
        "could",
        "couldnt",
        "did",
        "didnt",
        "do",
        "does",
        "doesnt",
        "doing",
        "dont",
        "down",
        "during",
        "each",
        "few",
        "for",
        "from",
        "further",
        "had",
        "hadnt",
        "has",
        "hasnt",
        "have",
        "havent",
        "having",
        "he",
        "hed",
        "hell",
        "hes",
        "her",
        "here",
        "heres",
        "hers",
        "herself",
        "him",
        "himself",
        "his",
        "how",
        "hows",
        "i",
        "id",
        "ill",
        "im",
        "ive",
        "if",
        "in",
        "into",
        "is",
        "isnt",
        "it",
        "its",
        "its",
        "itself",
        "lets",
        "me",
        "more",
        "most",
        "mustnt",
        "my",
        "myself",
        "no",
        "nor",
        "not",
        "of",
        "off",
        "on",
        "once",
        "only",
        "or",
        "other",
        "ought",
        "our",
        "ours",
        "ourselves",
        "out",
        "over",
        "own",
        "same",
        "shant",
        "she",
        "shed",
        "shell",
        "shes",
        "should",
        "shouldnt",
        "so",
        "some",
        "such",
        "than",
        "that",
        "thats",
        "the",
        "their",
        "theirs",
        "them",
        "themselves",
        "then",
        "there",
        "theres",
        "these",
        "they",
        "theyd",
        "theyll",
        "theyre",
        "theyve",
        "this",
        "those",
        "through",
        "to",
        "too",
        "under",
        "until",
        "up",
        "very",
        "was",
        "wasnt",
        "we",
        "wed",
        "well",
        "were",
        "weve",
        "were",
        "werent",
        "what",
        "whats",
        "when",
        "whens",
        "where",
        "wheres",
        "which",
        "while",
        "who",
        "whos",
        "whom",
        "why",
        "whys",
        "with",
        "wont",
        "would",
        "wouldnt",
        "you",
        "youd",
        "youll",
        "youre",
        "youve",
        "your",
        "yours",
        "yourself",
        "yourselves"
    };

    public string Code => "en-stop-words";

    public void FilterForSearch(
        List<SearchToken> tokens
    )
    {
        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (StopWords.Contains(PrepareTerm(token.Term)))
            {
                tokens.RemoveAt(i);
                i--;
            }
        }
    }

    public void FilterForIndex(
        List<IndexToken> tokens
    )
    {
        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (StopWords.Contains(PrepareTerm(token.Term)))
            {
                tokens.RemoveAt(i);
                i--;
            }
        }
    }

    private static string PrepareTerm(
        string term
    )
    {
        var stringBuilder = new StringBuilder();
        foreach (var c in term)
        {
            if (char.IsLetter(c))
            {
                stringBuilder.Append(char.ToLowerInvariant(c));
            }
        }

        return stringBuilder.ToString();
    }
}
