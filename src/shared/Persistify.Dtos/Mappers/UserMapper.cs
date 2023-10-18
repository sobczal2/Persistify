using Persistify.Domain.Users;
using Persistify.Dtos.Users;

namespace Persistify.Dtos.Mappers;

public static class UserMapper
{
    public static UserDto Map(User from)
    {
        return new UserDto { Username = from.Username, Permission = (int)from.Permission };
    }

    public static User Map(UserDto from)
    {
        return new User { Username = from.Username, Permission = (Permission)from.Permission };
    }
}
