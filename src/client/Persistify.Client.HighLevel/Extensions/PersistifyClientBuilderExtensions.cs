using Persistify.Client.HighLevel.Core;
using Persistify.Client.LowLevel.Core;

namespace Persistify.Client.HighLevel;

public static class PersistifyClientBuilderExtensions
{
    public static IPersistifyHighLevelClient BuildHighLevel(this PersistifyClientBuilder builder)
    {
        return new PersistifyHighLevelClient(builder.BuildLowLevel());
    }
}
