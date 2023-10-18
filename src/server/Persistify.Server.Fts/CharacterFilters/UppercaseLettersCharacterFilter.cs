using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterFilters;

public class UppercaseLettersCharacterFilter : ICharacterFilter
{
    public IEnumerable<char> AllowedCharacters { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
}
