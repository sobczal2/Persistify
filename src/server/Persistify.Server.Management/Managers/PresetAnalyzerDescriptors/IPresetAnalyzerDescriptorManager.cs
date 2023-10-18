using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.PresetAnalyzerDescriptors;

namespace Persistify.Server.Management.Managers.PresetAnalyzerDescriptors;

public interface IPresetAnalyzerDescriptorManager : IManager
{
    ValueTask<PresetAnalyzerDescriptor?> GetAsync(string presetName);
    bool Exists(string presetName);
    IAsyncEnumerable<PresetAnalyzerDescriptor> ListAsync(int take, int skip);
    void Add(PresetAnalyzerDescriptor preset);
    ValueTask<bool> RemoveAsync(string presetName);
}
