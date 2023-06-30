namespace Persistify.Server.Configuration.Settings;

public class AuthSettings
{
    public static string SectionName => "Auth";
    
    public bool ValidateIssuer { get; set; } = default!;
    public bool ValidateAudience { get; set; } = default!;
    public bool ValidateLifetime { get; set; } = default!;
    public bool ValidateIssuerSigningKey { get; set; } = default!;
    public string ValidIssuer { get; set; } = default!;
    public string ValidAudience { get; set; } = default!;
    public string IssuerSigningKey { get; set; } = default!;
}