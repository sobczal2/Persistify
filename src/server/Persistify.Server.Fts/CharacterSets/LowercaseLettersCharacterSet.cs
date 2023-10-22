using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterSets;

public class LowercaseLettersCharacterSet : ICharacterSet
{
    public IEnumerable<char> Characters => "abcdefghijklmnopqrstuvwxyz";
}
