using System.CommandLine;
using Persistify.Tools.Commands;

namespace Persistify.Tools;

public class PersistifyRootCommand : RootCommand
{
    public PersistifyRootCommand() : base("Persistify tools")
    {
        AddCommand(new GenerateProtosCommand());
    }
}
