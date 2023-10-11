using System.Collections.Generic;
using Persistify.Server.Fts.Analysis.Abstractions;

namespace Persistify.Server.Fts.Analysis.CharacterFilters;

public class LowercaseLettersCharacterFilter : ICharacterFilter
{
    public IEnumerable<char> AllowedCharacters { get; } = "abcdefghijklmnopqrstuvwxyz";
}
