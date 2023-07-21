namespace Persistify.Helpers.Common;

public class LogicHelpers
{
    public static bool Xor(bool a, bool b)
    {
        return (a || b) && !(a && b);
    }
}
