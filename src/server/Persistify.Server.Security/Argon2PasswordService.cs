using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Server.Security;

public class Argon2PasswordService : IPasswordService
{
    private readonly PasswordSettings _passwordSettings;

    public Argon2PasswordService(IOptions<PasswordSettings> passwordSettingsOptions)
    {
        _passwordSettings = passwordSettingsOptions.Value;
    }

    public (byte[] hash, byte[] salt) HashPassword(string password)
    {
        var salt = CreateSalt();
        var hash = HashPasswordInternal(password, salt);
        return (hash, salt);
    }

    public bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        var newHash = HashPasswordInternal(password, salt);
        return hash.SequenceEqual(newHash);
    }

    private byte[] HashPasswordInternal(string password, byte[] salt)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Iterations = _passwordSettings.Iterations,
            MemorySize = _passwordSettings.MemorySize,
            DegreeOfParallelism = _passwordSettings.Parallelism,
            Salt = salt
        };
        return argon2.GetBytes(_passwordSettings.HashSize);
    }

    private byte[] CreateSalt()
    {
        var salt = new byte[_passwordSettings.SaltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }
}
