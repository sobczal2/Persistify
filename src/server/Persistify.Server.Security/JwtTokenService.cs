using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Persistify.Server.Domain.Users;
using Persistify.Helpers.Time;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Server.Security;

public class JwtTokenService : ITokenService
{
    private readonly IClock _clock;
    private readonly TokenSettings _tokenSettings;

    public JwtTokenService(
        IOptions<TokenSettings> tokenSettingsOptions,
        IClock clock
    )
    {
        _clock = clock;
        _tokenSettings = tokenSettingsOptions.Value;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Id, user.Id.ToString()),
            new(ClaimTypes.Username, user.Username),
            new(ClaimTypes.Permission, ((int)user.Permission).ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _clock.UtcNow.Add(_tokenSettings.AccessTokenLifetime),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[_tokenSettings.RefreshTokenLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
