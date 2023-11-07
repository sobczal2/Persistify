using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterSets;

public class AsciiCharacterSet : ICharacterSet
{
    public string Code => "ascii";
    public IEnumerable<char> Characters => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
}
