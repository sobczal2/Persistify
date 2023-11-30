using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterSets;

public class EnglishLowercaseLettersCharacterSet : ICharacterSet
{
    public string Code => "en-lowercase-letters";
    public IEnumerable<char> Characters => "abcdefghijklmnopqrstuvwxyz";
}
