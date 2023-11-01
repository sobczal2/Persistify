using System.Collections.Generic;
using System.IO;
using Persistify.Server.Files;

namespace Persistify.Server.Management.Managers.Users;

public class UserManagerRequiredFileGroup : IRequiredFileGroup
{
    public static string IdentifierRepositoryFileName => Path.Join("User", "identifier.bin");
    public static string UserRepositoryMainFileName => Path.Join("User", "object.bin");

    public static string UserRepositoryOffsetLengthFileName =>
        Path.Join("User", "offsetLength.bin");

    public static string RefreshTokenRepositoryMainFileName =>
        Path.Join("RefreshToken", "object.bin");

    public static string RefreshTokenRepositoryOffsetLengthFileName =>
        Path.Join("RefreshToken", "offsetLength.bin");

    public string FileGroupName => "UserManager";

    public IEnumerable<string> GetFileNames()
    {
        return new List<string>
        {
            IdentifierRepositoryFileName,
            UserRepositoryMainFileName,
            UserRepositoryOffsetLengthFileName,
            RefreshTokenRepositoryMainFileName,
            RefreshTokenRepositoryOffsetLengthFileName
        };
    }
}
