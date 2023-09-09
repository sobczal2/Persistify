using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Users;

namespace Persistify.Server.Management.Managers.Users;

public interface IUserManager : IManager
{
    ValueTask<User?> GetAsync(string username);
    ValueTask<User?> GetAsync(int id);
    ValueTask<List<User>> ListAsync(int take, int skip);
    int Count();
    void Add(User user);
    ValueTask<bool> RemoveAsync(int id);
}
