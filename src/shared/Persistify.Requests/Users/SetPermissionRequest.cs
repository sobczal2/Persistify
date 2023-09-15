namespace Persistify.Requests.Users;

public class SetPermissionRequest
{
    public string Username { get; set; } = default!;
    public int Permission { get; set; }
}
