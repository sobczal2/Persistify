using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterSets;

public class EnglishUppercaseLettersCharacterSet : ICharacterSet
{
    public string Code => "en-uppercase-letters";
    public IEnumerable<char> Characters => "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
}
