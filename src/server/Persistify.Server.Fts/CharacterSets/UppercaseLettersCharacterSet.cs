using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterFilters;

public class UppercaseLettersCharacterSet : ICharacterSet
{
    public IEnumerable<char> Characters => "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
}
