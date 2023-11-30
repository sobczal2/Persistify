using System.Text.RegularExpressions;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterFilters;

public partial class HtmlStripCharacterFilter : ICharacterFilter
{
    public string Code => "htmlstrip";

    public string Filter(
        string value
    )
    {
        return HtmlStripRegex().Replace(value, string.Empty);
    }

    [GeneratedRegex("<.*?>")]
    private static partial Regex HtmlStripRegex();
}
