using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.PresetAnalyzerDescriptors;

namespace Persistify.Server.Management.Managers.PresetAnalyzerDescriptors;

public interface IPresetAnalyzerManager : IManager
{
    ValueTask<PresetAnalyzer?> GetAsync(string presetAnalyzerName);
    bool Exists(string presetName);
    IAsyncEnumerable<PresetAnalyzer> ListAsync(int take, int skip);
    void Add(PresetAnalyzer presetAnalyzer);
    ValueTask<bool> RemoveAsync(string presetAnalyzerName);
}
