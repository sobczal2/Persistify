using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterSets;

public class DigitsCharacterSet : ICharacterSet
{
    public IEnumerable<char> Characters => "0123456789";
}
