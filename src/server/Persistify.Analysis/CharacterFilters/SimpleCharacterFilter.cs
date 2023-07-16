using System.Text;

namespace Persistify.Analysis.CharacterFilters;

public class SimpleCharacterFilter : ICharacterFilter
{
    public string Filter(string text)
    {
        var builder = new StringBuilder(text.Length);
        foreach (var character in text)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToLowerInvariant(character));
            }
        }

        return builder.ToString();
    }
}
