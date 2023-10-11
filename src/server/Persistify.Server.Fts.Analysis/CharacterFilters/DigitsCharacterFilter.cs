using System.Collections.Generic;
using Persistify.Server.Fts.Analysis.Abstractions;

namespace Persistify.Server.Fts.Analysis.CharacterFilters;

public class DigitsCharacterFilter : ICharacterFilter
{
    public IEnumerable<char> AllowedCharacters { get; } = "0123456789";
}
