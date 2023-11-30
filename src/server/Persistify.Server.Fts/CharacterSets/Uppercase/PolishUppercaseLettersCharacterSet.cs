using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterSets;

public class PolishUppercaseLettersCharacterSet : ICharacterSet
{
    public string Code => "pl-uppercase-letters";
    public IEnumerable<char> Characters => "ABCDEFGHIJKLMNOPRSTUVWXYZĄĆĘŁŃÓŚŹŻ";
}
