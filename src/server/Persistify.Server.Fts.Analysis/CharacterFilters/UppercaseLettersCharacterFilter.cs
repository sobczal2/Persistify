using System.Collections.Generic;
using Persistify.Server.Fts.Analysis.Abstractions;

namespace Persistify.Server.Fts.Analysis.CharacterFilters;

public class UppercaseLettersCharacterFilter : ICharacterFilter
{
    public IEnumerable<char> AllowedCharacters { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
}
