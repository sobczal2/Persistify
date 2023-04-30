namespace Persistify.Stores.User;

public interface IUserStore
{
    void Create(string username, string password);
    void Delete(string username);
    bool Verify(string username, string password);
    bool Exists(string username);
    bool IsSuperUser(string username);
    string GenerateRefreshToken(string username);
    bool VerifyRefreshToken(string username, string refreshToken);
    string GenerateJwtToken(string username);
}