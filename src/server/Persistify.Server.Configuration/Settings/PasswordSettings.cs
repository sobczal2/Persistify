using Persistify.Server.Configuration.Enums;

namespace Persistify.Server.Configuration.Settings;

public class PasswordSettings
{
    public const string SectionName = "Password";

    public HashingAlgorithm Algorithm { get; set; }
    public int Iterations { get; set; }
    public int MemorySize { get; set; }
    public int Parallelism { get; set; }
    public int SaltSize { get; set; }
    public int HashSize { get; set; }
}
