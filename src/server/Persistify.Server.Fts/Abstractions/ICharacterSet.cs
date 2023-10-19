using System.Collections.Generic;

namespace Persistify.Server.Fts.Abstractions;

/// <summary>
///     Character filters are used to remove characters from the text before it is tokenized.
///     Analyzer constructs allowed alphabet from all character filters and uses it to filter out characters.
/// </summary>
public interface ICharacterSet
{
    IEnumerable<char> Characters { get; }
}
