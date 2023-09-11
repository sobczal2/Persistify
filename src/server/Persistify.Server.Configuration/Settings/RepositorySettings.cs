namespace Persistify.Server.Configuration.Settings;

public class RepositorySettings
{
    public const string SectionName = "Repository";

    public int TemplateRepositorySectorSize { get; set; }
    public int DocumentRepositorySectorSize { get; set; }
    public int UserRepositorySectorSize { get; set; }
    public int RefreshTokenRepositorySectorSize { get; set; }
}
