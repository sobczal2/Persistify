using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterSets;

public class PolishLowercaseLettersCharacterSet : ICharacterSet
{
    public string Code => "pl-lowercase-letters";
    public IEnumerable<char> Characters => "abcdefghijklmnopqrstuvwxyząćęłńóśźż";
}
