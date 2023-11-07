using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterSets;

public class AsciiSpecialCharacterSet : ICharacterSet
{
    public string Code => "ascii_special";
    public IEnumerable<char> Characters => "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
}
