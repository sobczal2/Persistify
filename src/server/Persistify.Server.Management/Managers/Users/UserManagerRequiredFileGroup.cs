﻿using System.Collections.Generic;
using System.IO;
using Persistify.Server.Management.Files;

namespace Persistify.Server.Management.Managers.Users;

public class UserManagerRequiredFileGroup : IRequiredFileGroup
{
    public static string IdentifierRepositoryFileName => Path.Join("User", "identifier.bin");
    public static string UserRepositoryMainFileName => Path.Join("User", "object.bin");
    public static string UserRepositoryOffsetLengthFileName => Path.Join("User", "offsetLength.bin");
    public string FileGroupName => "UserManager";

    public List<string> GetFileNames()
    {
        return new List<string>
        {
            IdentifierRepositoryFileName, UserRepositoryMainFileName, UserRepositoryOffsetLengthFileName
        };
    }
}