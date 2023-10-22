using Persistify.Client.Documents;
using Persistify.Client.PresetAnalyzers;
using Persistify.Client.Templates;
using Persistify.Client.Users;
using Persistify.Services;

namespace Persistify.Client.Core;

public interface IPersistifyClient
{
    IUsersClient Users { get; }
    ITemplatesClient Templates { get; }
    IDocumentsClient Documents { get; }
    IPresetAnalyzersClient PresetAnalyzerses { get; }
}
