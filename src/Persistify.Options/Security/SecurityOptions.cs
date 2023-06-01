namespace Persistify.Options;

public class SecurityOptions
{
    public const string SectionName = "Security";
    public class SOUser
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
    public class SOHash
    {
        public string Salt { get; set; } = default!;
        public int Iterations { get; set; }
        public int KeyLength { get; set; }
    }
    public class SOToken
    {
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string Key { get; set; } = default!;
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
        public int RefreshTokenLength { get; set; }
    }
    public SOUser SuperUser { get; set; } = default!;
    public SOHash Hash { get; set; } = default!;
    public SOToken Token { get; set; } = default!;
}