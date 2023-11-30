using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Domain.PresetAnalyzers;

namespace Persistify.Server.Management.Managers.PresetAnalyzers;

public interface IPresetAnalyzerManager : IManager
{
    ValueTask<PresetAnalyzer?> GetAsync(
        string presetAnalyzerName
    );

    bool Exists(
        string presetName
    );

    IAsyncEnumerable<PresetAnalyzer> ListAsync(
        int take,
        int skip
    );

    int Count();

    void Add(
        PresetAnalyzer presetAnalyzer
    );

    ValueTask<bool> RemoveAsync(
        string presetAnalyzerName
    );
}
