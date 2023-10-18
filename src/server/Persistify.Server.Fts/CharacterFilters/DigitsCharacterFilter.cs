using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterFilters;

public class DigitsCharacterFilter : ICharacterFilter
{
    public IEnumerable<char> AllowedCharacters { get; } = "0123456789";
}
