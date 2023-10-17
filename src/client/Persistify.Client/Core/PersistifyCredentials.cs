namespace Persistify.Client.Core;

public class PersistifyCredentials
{
    public PersistifyCredentials(string username, string password)
    {
        Username = username;
        Password = password;
    }

    internal string Username { get; }
    internal string Password { get; }

    internal string? AccessToken { get; set; }
    internal string? RefreshToken { get; set; }
}
