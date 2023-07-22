using Persistify.Domain.Fts;
using Persistify.Fts.Analysis.Abstractions;

namespace Persistify.Fts.Analysis.Analyzers;

public class StandardAnalyzer : IAnalyzer
{
    private readonly IEnumerable<ICharacterFilter> _characterFilters;
    private readonly ITokenizer _tokenizer;
    private readonly IEnumerable<ITokenFilter> _tokenFilters;

    public StandardAnalyzer(
        IEnumerable<ICharacterFilter> characterFilters,
        ITokenizer tokenizer,
        IEnumerable<ITokenFilter> tokenFilters
        )
    {
        _characterFilters = characterFilters;
        _tokenizer = tokenizer;
        _tokenFilters = tokenFilters;
    }
    public List<Token> Analyze(string text)
    {
        var filteredText = text;

        foreach (var characterFilter in _characterFilters)
        {
            filteredText = characterFilter.Filter(filteredText);
        }

        var tokens = _tokenizer.Tokenize(filteredText);

        foreach (var tokenFilter in _tokenFilters)
        {
            tokens = tokenFilter.Filter(tokens);
        }

        return tokens;
    }
}
