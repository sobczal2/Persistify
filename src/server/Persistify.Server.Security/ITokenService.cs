using Persistify.Server.Domain.Users;

namespace Persistify.Server.Security;

public interface ITokenService
{
    public string GenerateAccessToken(User user);
    public string GenerateRefreshToken();
}
