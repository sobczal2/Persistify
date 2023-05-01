using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Persistify.Stores.User;

public struct UserData
{
    public byte[] PasswordHash { get; set; }
    public (string Data, DateTime ValidTo)? RefreshToken { get; set; }

    public UserData(string password, string salt, int iterations, int keyLength)
    {
        var saltBytes = Encoding.UTF8.GetBytes(salt);
        PasswordHash = KeyDerivation.Pbkdf2(password, saltBytes, KeyDerivationPrf.HMACSHA256, iterations, keyLength);
    }

    public bool Verify(string password, string salt, int iterations, int keyLength)
    {
        var saltBytes = Encoding.UTF8.GetBytes(salt);
        var passwordHash =
            KeyDerivation.Pbkdf2(password, saltBytes, KeyDerivationPrf.HMACSHA256, iterations, keyLength);
        return passwordHash.SequenceEqual(PasswordHash);
    }

    public bool VerifyRefreshToken(string refreshToken)
    {
        if (RefreshToken == null)
            return false;
        if (RefreshToken.Value.ValidTo < DateTime.UtcNow)
        {
            RefreshToken = null;
            return false;
        }

        if (RefreshToken.Value.Data != refreshToken)
        {
            return false;
        }

        RefreshToken = null;
        return true;
    }

    public string CreateRefreshToken(int length, TimeSpan validFor)
    {
        var refreshToken = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(refreshToken);
        RefreshToken = (Convert.ToBase64String(refreshToken), DateTime.UtcNow + validFor);
        return RefreshToken.Value.Data;
    }
}