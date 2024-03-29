﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Domain.Users;

namespace Persistify.Server.Management.Managers.Users;

public interface IUserManager : IManager
{
    ValueTask<User?> GetAsync(
        string username
    );

    ValueTask<User?> GetAsync(
        int id
    );

    bool Exists(
        string username
    );

    IAsyncEnumerable<User> ListAsync(
        int take,
        int skip
    );

    int Count();

    void Add(
        User user
    );

    ValueTask<bool> RemoveAsync(
        int id
    );

    ValueTask<(string accessToken, string refreshToken)> CreateTokens(
        int id
    );

    ValueTask<bool> CheckRefreshToken(
        int id,
        string refreshToken
    );

    ValueTask Update(
        User user
    );
}
