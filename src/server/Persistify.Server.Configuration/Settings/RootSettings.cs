namespace Persistify.Server.Configuration.Settings;

public class RootSettings
{
    public const string SectionName = "Root";

    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
