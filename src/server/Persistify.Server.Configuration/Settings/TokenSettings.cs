using System;
using Persistify.Server.Configuration.Enums;

namespace Persistify.Server.Configuration.Settings;

public class TokenSettings
{
    public const string SectionName = "Token";

    public TokenType Type { get; set; }
    public string Secret { get; set; } = default!;
    public TimeSpan AccessTokenLifetime { get; set; }
    public TimeSpan RefreshTokenLifetime { get; set; }
    public int RefreshTokenLength { get; set; }
}
