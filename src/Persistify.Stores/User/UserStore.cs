using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Persistify.Options;
using Persistify.Storage;
using Persistify.Stores.Common;

namespace Persistify.Stores.User;

public class UserStore : IUserStore, IPersisted
{
    private readonly SecurityOptions _securityOptions;
    private (string Username, UserData Data) _superUser;
    private ConcurrentDictionary<string, UserData> _users = default!;

    public UserStore(IOptions<SecurityOptions> securityOptions)
    {
        _securityOptions = securityOptions.Value;
        _superUser = (_securityOptions.SuperUser.Username,
            new UserData(_securityOptions.SuperUser.Password,
                _securityOptions.Hash.Salt,
                _securityOptions.Hash.Iterations,
                _securityOptions.Hash.KeyLength
            ));
    }

    public async ValueTask LoadAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        var exists = await storage.ExistsBlobAsync("users", cancellationToken);
        if (exists)
        {
            var result = await storage.LoadBlobAsync("users", cancellationToken);
            _users = JsonConvert.DeserializeObject<ConcurrentDictionary<string, UserData>>(result)!;
            return;
        }

        _users = new ConcurrentDictionary<string, UserData>();
    }

    public async ValueTask SaveAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        AssertInitialized();
        await storage.SaveBlobAsync("users", JsonConvert.SerializeObject(_users), cancellationToken);
    }

    public void Create(string username, string password)
    {
        AssertInitialized();
        if (!_users.TryAdd(username,
                new UserData(password,
                    _securityOptions.Hash.Salt,
                    _securityOptions.Hash.Iterations,
                    _securityOptions.Hash.KeyLength
                )))
            throw new StoreException("User already exists");
    }

    public void Delete(string username)
    {
        AssertInitialized();
        if (!_users.TryRemove(username, out _))
            throw new StoreException("User not found");
    }

    public bool Verify(string username, string password)
    {
        AssertInitialized();
        if (_superUser.Username == username)
            return _superUser.Data.Verify(password,
                _securityOptions.Hash.Salt,
                _securityOptions.Hash.Iterations,
                _securityOptions.Hash.KeyLength
                );
        if (!_users.TryGetValue(username, out var userData))
            return false;
        return userData.Verify(password,
            _securityOptions.Hash.Salt,
            _securityOptions.Hash.Iterations,
            _securityOptions.Hash.KeyLength
            );
    }

    public bool Exists(string username)
    {
        AssertInitialized();
        return _users.ContainsKey(username) || _superUser.Username == username;
    }

    public bool IsSuperUser(string username)
    {
        AssertInitialized();
        return _superUser.Username == username;
    }

    public string GenerateRefreshToken(string username)
    {
        if (_superUser.Username == username)
            return _superUser.Data.CreateRefreshToken(
                _securityOptions.Token.RefreshTokenLength,
                TimeSpan.FromMinutes(_securityOptions.Token.RefreshTokenExpiration));

        if (!_users.TryGetValue(username, out var userData))
            throw new StoreException("User not found");

        return userData.CreateRefreshToken(
            _securityOptions.Token.RefreshTokenLength,
            TimeSpan.FromMinutes(_securityOptions.Token.RefreshTokenExpiration));
    }

    public bool VerifyRefreshToken(string username, string refreshToken)
    {
        if (_superUser.Username == username)
            return _superUser.Data.VerifyRefreshToken(refreshToken);

        if (!_users.TryGetValue(username, out var userData))
            throw new StoreException("User not found");

        return userData.VerifyRefreshToken(refreshToken);
    }

    public string GenerateJwtToken(string username)
    {
        var exists = Exists(username);
        if (!exists)
            throw new StoreException("User not found");
        var isSuperUser = IsSuperUser(username);
        var claims = isSuperUser
            ? new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, UserRoles.SuperUser)
            }
            : new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

        var token = new JwtSecurityToken(
            issuer: _securityOptions.Token.Issuer,
            audience: _securityOptions.Token.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_securityOptions.Token.AccessTokenExpiration),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityOptions.Token.Key)),
                SecurityAlgorithms.HmacSha512Signature
            )
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private void AssertInitialized()
    {
        if (_users == null)
            throw new StoreException("Store not initialized");
    }
}