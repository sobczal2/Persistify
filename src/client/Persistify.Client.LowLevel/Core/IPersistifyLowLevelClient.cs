using Persistify.Client.LowLevel.Documents;
using Persistify.Client.LowLevel.PresetAnalyzers;
using Persistify.Client.LowLevel.Templates;
using Persistify.Client.LowLevel.Users;

namespace Persistify.Client.LowLevel.Core;

public interface IPersistifyLowLevelClient
{
    IUsersClient Users { get; }
    ITemplatesClient Templates { get; }
    IDocumentsClient Documents { get; }
    IPresetAnalyzersClient PresetAnalyzers { get; }
}
