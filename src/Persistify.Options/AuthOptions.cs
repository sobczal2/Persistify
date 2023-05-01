using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Persistify.Options;

public class AuthOptions
{
    public const string SectionName = "Auth";
    [Required, MinLength(4)]
    public string SuUsername { get; set; } = default!;
    [Required, MinLength(8)]
    public string SuPassword { get; set; } = default!;
    [Required]
    public string Salt { get; set; } = default!;
    [Required]
    public int Iterations { get; set; }
    [Required]
    public int KeyLength { get; set; }
    [Required]
    public int RefreshTokenLength { get; set; }
    [Required]
    public string Issuer { get; set; } = default!;
    [Required]
    public double JwtTokenExpirationMinutes { get; set; }
    [Required]
    public double RefreshTokenExpirationMinutes { get; set; }
    [Required, MinLength(32)]
    public string JwtSecret { get; set; } = default!;

    public SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.ASCII.GetBytes(JwtSecret);
        return new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);
    }
}